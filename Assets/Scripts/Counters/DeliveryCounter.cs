using KitchenChaos.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenChaos.Interactions
{
    public class DeliveryCounter : BaseCounter
    {
        public override void Interact(PlayerInteractions player)
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plate))
                {
                    DeliveryManager.Instance.DeliverRecipe(plate);

                    player.GetKitchenObject().DestroySelf();
                }
            }
        }
    }
}
