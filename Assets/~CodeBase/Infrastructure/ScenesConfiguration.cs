using System;
using System.Linq;
using _CodeBase.Infrastructure.GameStructs;
using UnityEngine;

namespace _CodeBase.Infrastructure
{
    [CreateAssetMenu(fileName = nameof(ScenesConfiguration), menuName = ConfigPathOrigin.MainHub + nameof(ScenesConfiguration))]
    public sealed class ScenesConfiguration : ScriptableObject, IConfiguration
    {
        [SerializeField] private GameSceneBinder[] _gameSceneBinders;

        public string GetPath(GameScene gameScene)
            => _gameSceneBinders.First(sb => sb._scene == gameScene)._originPath;
        
    }


    [Serializable] internal sealed class GameSceneBinder
    {
        public GameScene _scene;
        public string _originPath;
    } 
}