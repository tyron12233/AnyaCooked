using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace KitchenChaos.Interactions.Multiplayer
{
    public class GameMultiplayer : NetworkBehaviour
    {
        public static GameMultiplayer Instance { get; private set; }

        [SerializeField] SO_KitchenObjectList _kitchenObjectListSO;

        void Awake()
        {
            Instance = this;
        }

        //for prototyping only - implement pooling
        public void SpawnKitchenObject(SO_KitchenObject kitchenObjectSO, IKitchenObjectHolder kitchenObjectHolder)
        {
            int kitchenObjectSOIndex = GetKitchenObjectSOIndex(kitchenObjectSO);

            SpawnKitchenObjectServerRpc(kitchenObjectSOIndex, kitchenObjectHolder.GetNetworkObject());
        }

        // Only server can spawn networkObjects so serverRpc is needed,
        // but we want client to be able to call this function.
        // Turned SO_KitchenObject to int and IKitchenObjectHolder to NetworkObjectReference
        // for serialization purposes.
        [ServerRpc(RequireOwnership = false)]
        void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectHolder_NetworkObjRef)
        {
            SO_KitchenObject kitchenObjectSO = GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

            Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.KitchenObjectPrefab);

            // Spawn object on network
            NetworkObject kitchenObject_NetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
            kitchenObject_NetworkObject.Spawn(true);

            // Set KitchenObject holder/parent
            KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

            kitchenObjectHolder_NetworkObjRef.TryGet(out NetworkObject kitchenObjectHolder_NetworkObject);
            IKitchenObjectHolder kitchenObjectHolder = kitchenObjectHolder_NetworkObject.GetComponent<IKitchenObjectHolder>();

            kitchenObject.SetKitchenObjectHolder(kitchenObjectHolder);
        }

        int GetKitchenObjectSOIndex(SO_KitchenObject kitchenObjectSO)
        {
            return _kitchenObjectListSO.KitchenObjectList.IndexOf(kitchenObjectSO);
        }

        SO_KitchenObject GetKitchenObjectSOFromIndex(int kitchenObjectSOIndex)
        {
            return _kitchenObjectListSO.KitchenObjectList[kitchenObjectSOIndex];
        }
    }
}
