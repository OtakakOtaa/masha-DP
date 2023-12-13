using System.Linq;
using _CodeBase.Customers._Data;
using _CodeBase.Infrastructure;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace _CodeBase.Customers
{
    public sealed class CustomerFetcher : MonoBehaviour
    {
        [SerializeField] private CustomersConfiguration _configuration;
        [SerializeField] private Customer _customerPrefab;
        [SerializeField] private TimelineAsset _customerTimeline;
        [SerializeField] private PlayableDirector _playableDirector;
        
        private Customer _target;
        
        
        public async UniTaskVoid PerformNextCustomer()
        {
            var customer = GetNext();
            _playableDirector.playableAsset = _customerTimeline;
            _playableDirector.Play();
        }
        
        private Customer GetNext()
        {
            _target ??= CreateCustomer();
            return _target = BuildCustomer();
        }

        private Customer CreateCustomer()
        {
            var customer = Instantiate(_customerPrefab);
            var animationTrack = _customerTimeline.GetOutputTrack(0) as AnimationTrack;
            _playableDirector.SetGenericBinding(animationTrack, customer.Animator);
            return customer;
        }
        

        private Customer BuildCustomer()
        {
            return _target.Init
            (
                _configuration.Sprites.GetRandom(),
                _configuration.Orders.GetRandom(),
                _configuration.CustomerInfos.GetRandom()
            );
        }
    }
}