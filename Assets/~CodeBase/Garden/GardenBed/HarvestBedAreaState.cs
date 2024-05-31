﻿using System.Linq;
using _CodeBase.Input.Manager;

namespace _CodeBase.Garden.GardenBed
{
    public sealed class HarvestBedAreaState : IGardenBedAreaState
    {
        private readonly GardenBedArea _gardenBedArea;

        
        public GardenBedArea.State CurrentState =>GardenBedArea.State.NeedHarvest;


        public HarvestBedAreaState(GardenBedArea gardenBedArea)
        {
            _gardenBedArea = gardenBedArea;
        }
        
        
        public void SwitchState(GardenBedArea.State newState)
        {
            foreach (var cell in _gardenBedArea.Cells)
            {
                cell.LockFlag = false;
            }
        }

        
        public void ProcessInteractivity(InputManager.InputAction inputAction)
        {
        }

        
        public void UpdateState()
        {
            if (!_gardenBedArea.Cells.All(c => c.HasPlant is false)) return;
            if (_gardenBedArea.ProblemTimer.IsTimeUp is false)
            {
                _gardenBedArea.SwitchState(GardenBedArea.State.ReadyToUsing);
            }
            else
            {
                _gardenBedArea.ExecuteProblem();
            }
        }
    }
}