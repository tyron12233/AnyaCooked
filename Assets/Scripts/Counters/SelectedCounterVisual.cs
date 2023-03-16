using KitchenChaos.Control;
using UnityEngine;

namespace KitchenChaos.Interactions.Visual
{
    public class SelectedCounterVisual : MonoBehaviour
    {
        [SerializeField] BaseCounter _counter;
        [SerializeField] GameObject _selectedCounterVisual;

        void Start()
        {
            //PlayerController.Instance.OnSelectedCounterChanged += PlayerController_OnSelectedCounterChanged;
        }

        void PlayerController_OnSelectedCounterChanged(BaseCounter selectedCounter)
        {
            DisplaySelection(selectedCounter == _counter);
        }

        void DisplaySelection(bool shouldDisplay)
        {
            _selectedCounterVisual.SetActive(shouldDisplay);
        }

        void OnDestroy()
        {
            //PlayerController.Instance.OnSelectedCounterChanged -= PlayerController_OnSelectedCounterChanged;
        }
    }
}
