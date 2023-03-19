using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace KitchenChaos.Interactions
{
    public class PlayerInteractions : NetworkBehaviour, IKitchenObjectHolder
    {
        public static event Action<PlayerInteractions> OnPickedSomething;
        
        [SerializeField] Transform _kitchenObjectHoldPoint;

        KitchenObject _kitchenObject;

        public void ClearKitchenObject()
        {
            _kitchenObject = null;
        }

        public KitchenObject GetKitchenObject()
        {
            return _kitchenObject;
        }

        public Transform GetKitchenObjectSpawnPoint()
        {
            return _kitchenObjectHoldPoint;
        }

        public bool HasKitchenObject()
        {
            return _kitchenObject != null;
        }

        public void SetKitchenObject(KitchenObject kitchenObject)
        {
            _kitchenObject = kitchenObject;

            if (_kitchenObject != null)
                OnPickedSomething?.Invoke(this); //prep for multiplayer
        }

        public static void ResetStaticData()
        {
            OnPickedSomething = null;
        }

        public NetworkObject GetNetworkObject()
        {
            // since this class is a NetworkBehaviour,
            // we can just retrieve the attached NetworkObject
            return NetworkObject;
        }
    }
}
