using System;
using System.Linq;
using _CodeBase.DATA;
using _CodeBase.Infrastructure;
using DG.Tweening.Plugins.Core.PathCore;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Path = System.IO.Path;

namespace _CodeBase.Customers._Data
{
    [Serializable] public sealed class CustomerVisual : PollEntity, IUniq
    {
        [SerializeField] private string _id;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private Color _mainColor = Color.white;
        [SerializeField] private GifInfo _sadTalkAnim;
        [SerializeField] private GifInfo _happyTalkAnim;
        
        
        public override Sprite Sprite => _sprite;
        public override string ID => _id;
        public override string Name => _id;
        public Color MainColor => _mainColor;
        public override UniqItemsType Type => UniqItemsType.CustomerVisual;

        public GifInfo SadTalkAnim => _sadTalkAnim;
        public GifInfo HappyTalkAnim => _happyTalkAnim;

#if UNITY_EDITOR
        public void SetID_EDITOR(string id)
        {
            _id = id;
        }


        [Button("Detect Talk Gif And Init With Auto name")]
        private void InitTalkGif()
        {
            InitTalkGif(_sprite.name);
        }
        
        [Button("Detect Talk Gif And Init")]
        private void InitTalkGif(string pedName)
        {
            _sadTalkAnim = null;
            var sadPath = Path.Combine(pedName, "sad.gif");
            var happyPath = Path.Combine(pedName, "talk&happy.gif");
            
            _sadTalkAnim = GifAnimationDecoder.Process(sadPath, $"Assets/sad_{pedName}");
            _happyTalkAnim = GifAnimationDecoder.Process(happyPath, $"Assets/happy_{pedName}");
        }
        
        [Button("Detect precisely Sad Talk Gif And Init")]
        private void InitPreciselySTalkGif(string gifName)
        {
            _sadTalkAnim = null;
            _sadTalkAnim = GifAnimationDecoder.Process(gifName, $"Assets/sad_{gifName}");
        }
        
        [Button("Detect precisely Happy Talk Gif And Init")]
        private void InitPreciselyHTalkGif(string gifName)
        {
            _happyTalkAnim = null;
            _happyTalkAnim = GifAnimationDecoder.Process(gifName, $"Assets/happy_{gifName}");
        }
#endif
        
    }
}