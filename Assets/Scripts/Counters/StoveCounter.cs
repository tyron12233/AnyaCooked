using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenChaos.Interactions
{
    public class StoveCounter : BaseCounter, IHasProgress
    {
        public static event Action<State, StoveCounter> OnStateChanged;
        public event Action<float> OnProgressChanged;

        public enum State
        {
            Idle,
            Frying,
            Fried,
            Burned,
        }

        State _state;
        SO_FryingRecipe _currentFryingRecipe;
        SO_BurningRecipe _currentBurningRecipe;
        float _fryingTimer;
        float _burningTimer;

        void Start()
        {
            _state = State.Idle;
        }

        void Update()
        {
            if (!HasKitchenObject()) return;

            //SM pattern instead!
            switch (_state)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    ExecuteFrying();
                    break;
                case State.Fried:
                    ExecuteBurning();
                    break;
                case State.Burned:
                    OnProgressChanged?.Invoke(0);
                    break;
            }
        }

        void ExecuteBurning()
        {
            _burningTimer += Time.deltaTime;
            OnProgressChanged?.Invoke(GetProgressNormalized(_burningTimer, _currentBurningRecipe.BurningTimerMax));

            if (_burningTimer > GetBurningTimerMax())
            {
                GetKitchenObject().DestroySelf();
                KitchenObject.SpawnKitchenObject(_currentBurningRecipe.Output, this);
                _currentBurningRecipe = null;

                _state = State.Burned;
                OnStateChanged?.Invoke(_state, this);
            }
        }

        void ExecuteFrying()
        {
            _fryingTimer += Time.deltaTime;
            OnProgressChanged?.Invoke(GetProgressNormalized(_fryingTimer, _currentFryingRecipe.FryingTimerMax));

            if (_fryingTimer > GetFryingTimerMax())
            {
                GetKitchenObject().DestroySelf();
                KitchenObject.SpawnKitchenObject(_currentFryingRecipe.Output, this);
                _currentFryingRecipe = null;

                _state = State.Fried;
                _currentBurningRecipe = GetKitchenObject().KitchenObjectSO.GetBurningRecipe();
                _burningTimer = 0f;
                OnStateChanged?.Invoke(_state, this);
            }
        }

        public override void Interact(PlayerInteractions player)
        {
            if (!HasKitchenObject())
            {
                if (player.HasKitchenObject())
                {
                    KitchenObject playerKitchenObject = player.GetKitchenObject();
                    if (CanBeFried(playerKitchenObject))
                    {
                        playerKitchenObject.SetKitchenObjectHolder(this);
                        _currentFryingRecipe = GetKitchenObject().KitchenObjectSO.GetFryingRecipe();

                        _fryingTimer = 0f;
                        _state = State.Frying;
                        OnStateChanged?.Invoke(_state, this);

                        OnProgressChanged?.Invoke(GetProgressNormalized(_fryingTimer, _currentFryingRecipe.FryingTimerMax));
                        //if (_currentKitchenObject.CuttingTracker != 0)
                        //    OnProgressChanged?.Invoke(TrackCuts());
                    }
                }
            }
            else
            {
                if (!player.HasKitchenObject())
                {
                    GetKitchenObject().SetKitchenObjectHolder(player);
                    CleanStove();
                }
                else
                {
                    if (AttemptTransferFromCounterToPlate(player))
                        CleanStove();
                }
            }
        }

        bool CanBeFried(KitchenObject kitchenObject)
        {
            return kitchenObject.KitchenObjectSO.GetFryingRecipe() != null;
        }

        public bool IsFried()
        {
            return _state == State.Fried;
        }

        float GetFryingTimerMax()
        {
            return _currentFryingRecipe.FryingTimerMax;
        }

        float GetBurningTimerMax()
        {
            return _currentBurningRecipe.BurningTimerMax;
        }

        void CleanStove()
        {
            _currentFryingRecipe = null;
            _currentBurningRecipe = null;
            _state = State.Idle;

            OnStateChanged?.Invoke(_state, this);
            OnProgressChanged?.Invoke(0);
        }

        float GetProgressNormalized(float timer, float max)
        {
            return timer / max;
        }

        new public static void ResetStaticData()
        {
            OnStateChanged = null;
        }
    }
}
