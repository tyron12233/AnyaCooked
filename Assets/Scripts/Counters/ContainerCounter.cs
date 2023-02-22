using System;
using UnityEngine;

namespace KitchenChaos.Interactions
{
    public class ContainerCounter : BaseCounter
    {
        public event Action OnPlayerGrabbedObject;

        [SerializeField] SO_KitchenObject _kitchenObjectSO;

        public override void Interact(PlayerInteractions player)
        {
            if (!HasKitchenObject())
            {
                if (player.HasKitchenObject())
                    player.GetKitchenObject().SetKitchenObjectHolder(this);
                else
                {
                    KitchenObject.SpawnKitchenObject(_kitchenObjectSO, player);
                    OnPlayerGrabbedObject?.Invoke();
                }
            }
            else
            {
                if (!player.HasKitchenObject())
                    GetKitchenObject().SetKitchenObjectHolder(player);
                else
                {
                    if (!AttemptTransferFromCounterToPlate(player))
                        AttemptTransferFromPlayerToPlate(player);
                }
            }               
        }
    }
}

