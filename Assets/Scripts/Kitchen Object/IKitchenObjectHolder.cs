using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenChaos.Interactions
{
    public interface IKitchenObjectHolder 
    {
        Transform GetKitchenObjectSpawnPoint();
        void SetKitchenObject(KitchenObject kitchenObject);
        KitchenObject GetKitchenObject();
        void ClearKitchenObject();
        bool HasKitchenObject();
    }
}
