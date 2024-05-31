using System.Collections.Generic;
using System.Linq;
using _CodeBase.Infrastructure;
using _CodeBase.Infrastructure.GameStructs.FSM;
using _CodeBase.Infrastructure.GameStructs.FSM.States;
using _CodeBase.Input;
using _CodeBase.MainMenu;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using VContainer;

namespace _CodeBase
{
    public sealed class GameService : MonoBehaviour, IGameState
    {
        private const string SavesId = "saves_1"; 
        
        
        [FormerlySerializedAs("_scenesConfiguration")] [SerializeField] private SceneResolver _sceneResolver;

        private static readonly ReactiveCommand GameUpdateSource = new();
        public static IReactiveCommand<Unit> GameUpdate => GameUpdateSource;

        
        [Inject] private readonly GlobalStateMachine _gameStateMachine;
        [Inject] private readonly GameplayCursor _gameplayCursor;
        [Inject] private readonly GameConfigProvider _gameplayConfigProvider;
        
        private readonly HashSet<GameScene> _currentActiveAdditiveScenes = new();
        public IEnumerable<GameScene> CurrentActiveAdditiveScenes => _currentActiveAdditiveScenes;
        public GlobalStateMachine GameStateMachine => _gameStateMachine;
        public PersistentGameData PersistentGameData { get; private set; }


        // EntryPoint //
        public void Start()
        { 
            DontDestroyOnLoad(this);
            DontDestroyOnLoad(_gameplayCursor);
            
            Enter();
        }

        public void Update()
        {
            GameUpdateSource.Execute(); 
        }

        public async void Enter()
        { 
            InitSaves();            
            await TryLoadScene(GameScene.MainMenu);
            _gameStateMachine.Enter<MainMenuGameState>();
        }


        public async UniTask TryLoadScene(GameScene scene, bool asAdditive = false)
        {
            if (_currentActiveAdditiveScenes.Contains(scene))
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(_sceneResolver[scene]));
                return;
            }

            if (asAdditive)
            {
                _currentActiveAdditiveScenes.Add(scene);
            }
            else
            {
                _currentActiveAdditiveScenes.Clear();
            }
            
            
            await SceneManager.LoadSceneAsync(_sceneResolver[scene], asAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_sceneResolver[scene]));
        }

        public async UniTask TryReleaseAdditiveScene(GameScene targetScene, GameScene activeScene)
        {
            if(!_currentActiveAdditiveScenes.Contains(targetScene)) return;
            
            _currentActiveAdditiveScenes.Remove(targetScene);
            await SceneManager.UnloadSceneAsync(_sceneResolver[targetScene]);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_sceneResolver[activeScene]));
        }

        public void SaveGameData()
        {
            PlayerPrefs.SetString(SavesId, JsonUtility.ToJson(PersistentGameData));
        }
        
        private void InitSaves()
        {
            var rawSaves = PlayerPrefs.GetString(SavesId, string.Empty);
            PersistentGameData = string.IsNullOrEmpty(rawSaves) ? _gameplayConfigProvider.StaticData : JsonUtility.FromJson<PersistentGameData>(rawSaves);
        }
    }
}