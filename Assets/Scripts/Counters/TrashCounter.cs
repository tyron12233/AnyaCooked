using System;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenChaos.Interactions
{
    public class TrashCounter : BaseCounter
    {
        public static Action<TrashCounter> OnAnyObjectTrashed;

        //prototype code - implement pooling
        public override void Interact(PlayerInteractions player)
        {
            if (player.HasKitchenObject())
            {
                OnAnyObjectTrashed?.Invoke(this);
                player.GetKitchenObject().DestroySelf();
            }
        }

        new public static void ResetStaticData()
        {
            OnAnyObjectTrashed = null;
        }
    }
}
