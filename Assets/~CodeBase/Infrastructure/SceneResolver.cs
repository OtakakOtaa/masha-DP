using System;
using System.Collections.Generic;
using System.Linq;
using _CodeBase.Infrastructure.GameStructs;
using UnityEngine;
using UnityEngine.Serialization;

namespace _CodeBase.Infrastructure
{
    [CreateAssetMenu(fileName = nameof(SceneResolver), menuName = ConfigPathOrigin.MainHub + nameof(SceneResolver))]
    public sealed class SceneResolver : ScriptableObject, IConfiguration
    {
        [SerializeField] private GameSceneBinder[] _gameSceneBinders;

        private Dictionary<GameScene, string> _cashedMap;
        
        public string this[GameScene gameScene] 
        {
            get
            {
                _cashedMap ??= _gameSceneBinders.ToDictionary(b => b.scene, b => b.originPath);
                return _cashedMap[gameScene];
            }
        }
    }


    [Serializable] internal sealed class GameSceneBinder
    {
        [FormerlySerializedAs("_scene")] public GameScene scene;
        [FormerlySerializedAs("_originPath")] public string originPath;
    } 
}