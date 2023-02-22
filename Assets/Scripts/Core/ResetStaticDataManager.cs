using UnityEngine;
using KitchenChaos.Interactions;
using KitchenChaos.Core.UI;

namespace KitchenChaos.Core
{
    public class ResetStaticDataManager : MonoBehaviour
    {
        private void Awake()
        {
            PlayerInteractions.ResetStaticData();
            BaseCounter.ResetStaticData();
            CuttingCounter.ResetStaticData();
            StoveCounter.ResetStaticData();
            TrashCounter.ResetStaticData();
            StartCountdownUI.ResetStaticData();
        }
    }
}
