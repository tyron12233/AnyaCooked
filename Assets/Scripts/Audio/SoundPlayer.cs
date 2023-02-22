using UnityEngine;
using KitchenChaos.Core;
using KitchenChaos.Interactions;
using KitchenChaos.Core.UI;

namespace KitchenChaos.Audio
{
    public class SoundPlayer : MonoBehaviour
    {
        [SerializeField] SO_SoundConfig _soundConfig;
        public SO_SoundConfig SoundConfig => _soundConfig;

        [Header("Sound world position")]
        [SerializeField] Transform _deliveryCounter; //refactor this to allow for more

        void Start()
        {
            DeliveryManager.Instance.OnRecipeDelivered += DeliveryManager_OnRecipeDelivered;
            DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
            CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
            PlayerInteractions.OnPickedSomething += PlayerInteractions_OnPickedSomething;
            BaseCounter.OnAnyObjectPlaced += BaseCounter_OnAnyObjectPlaced;
            TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
            StoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
            StartCountdownUI.OnNumberChanged += StartCountdownUI_OnNumberChanged;
        }

        void StartCountdownUI_OnNumberChanged()
        {
            _soundConfig.PlaySingleSound(SoundType.Warning, Vector3.zero);
        }

        void StoveCounter_OnStateChanged(StoveCounter.State state, StoveCounter counter)
        {
            bool shouldPlay = state == StoveCounter.State.Frying || state == StoveCounter.State.Fried;

            if (shouldPlay)
                _soundConfig.PlayLoopingSound(SoundType.StoveSizzle, counter.gameObject);
            else
                _soundConfig.StopPlayingSound(counter.gameObject);
        }

        void TrashCounter_OnAnyObjectTrashed(TrashCounter counter)
        {
            _soundConfig.PlaySingleSound(SoundType.Trash, counter.transform.position);
        }

        void BaseCounter_OnAnyObjectPlaced(BaseCounter counter)
        {
            _soundConfig.PlaySingleSound(SoundType.ObjectDrop, counter.transform.position);
        }

        void PlayerInteractions_OnPickedSomething(PlayerInteractions player)
        {
            _soundConfig.PlaySingleSound(SoundType.ObjectPickup, player.transform.position);
        }

        void CuttingCounter_OnAnyCut(CuttingCounter counter)
        {
            _soundConfig.PlaySingleSound(SoundType.Chop, counter.transform.position);
        }

        void DeliveryManager_OnRecipeDelivered()
        {
            _soundConfig.PlaySingleSound(SoundType.DeliverySuccess, _deliveryCounter.position);
        }

        void DeliveryManager_OnRecipeFailed()
        {
            _soundConfig.PlaySingleSound(SoundType.DeliveryFail, _deliveryCounter.position);
        }

        private void OnDestroy()
        {
            DeliveryManager.Instance.OnRecipeDelivered -= DeliveryManager_OnRecipeDelivered;
            DeliveryManager.Instance.OnRecipeFailed -= DeliveryManager_OnRecipeFailed;
            CuttingCounter.OnAnyCut -= CuttingCounter_OnAnyCut;
            PlayerInteractions.OnPickedSomething -= PlayerInteractions_OnPickedSomething;
            BaseCounter.OnAnyObjectPlaced -= BaseCounter_OnAnyObjectPlaced;
            TrashCounter.OnAnyObjectTrashed -= TrashCounter_OnAnyObjectTrashed;
            StoveCounter.OnStateChanged -= StoveCounter_OnStateChanged;
            StartCountdownUI.OnNumberChanged -= StartCountdownUI_OnNumberChanged;
        }
    }
}
