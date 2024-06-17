using System.Linq;
using _CodeBase.Input.Manager;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _CodeBase.Garden.GardenBed
{
    public sealed class BedAreaGrowingState : IGardenBedAreaState
    {
        private readonly GardenBedArea _gardenBedArea;
        private float _currentMaxGrowCellOffset;
        
        
        public GardenBedArea.State CurrentState => GardenBedArea.State.Growing;


        public BedAreaGrowingState(GardenBedArea gardenBedArea)
        {
            _gardenBedArea = gardenBedArea;
        }

        
        public void UpdateState()
        {
            foreach (var cell in _gardenBedArea.Cells)
            {
                if (cell.HasPlant)
                {
                    cell.UpdateProgress();
                }
            }

            _gardenBedArea.UI.UpdateProgressBar(_gardenBedArea.GrowTimer.TimeRatio);

            if (_gardenBedArea.Cells.All(c => c.Progress >= 1f))
            {
                _gardenBedArea.SwitchState(GardenBedArea.State.NeedHarvest);
            }
        }
        
        public void SwitchState(GardenBedArea.State newState)
        {
            _gardenBedArea.AudioService.PlayEffect(_gardenBedArea.GrowAudio);
            _currentMaxGrowCellOffset = 0f;
                
            foreach (var cell in _gardenBedArea.Cells)
            {
                cell.LockFlag = true;
                    
                var growingTimeOffset = Random.Range(_gardenBedArea.GrowingStartRandomOffsetRange.x, _gardenBedArea.GrowingStartRandomOffsetRange.y);
                if (_currentMaxGrowCellOffset < growingTimeOffset) _currentMaxGrowCellOffset = growingTimeOffset; 
                    
                UniTask.Delay((int)(growingTimeOffset * 1000), cancellationToken: _gardenBedArea.DestroyCancellationToken)
                    .ContinueWith(() => cell.ApplyGrownPlantState(_gardenBedArea.PlantConfig, _gardenBedArea.NeedConsedStartRandomOffset ? -growingTimeOffset : 0f));
            }
            
            _gardenBedArea.GrowTimer.RunWithDuration((float)(_gardenBedArea.PlantConfig.GrowTime + _currentMaxGrowCellOffset));
            _gardenBedArea.UI.ShowProgressBar();
        }

        public void ProcessInteractivity(InputManager.InputAction inputAction)
        {
        }
    }
}