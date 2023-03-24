using Unity.Netcode;
using UnityEngine;
using KitchenChaos.Core;

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

        public void StartHost()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
            NetworkManager.Singleton.StartHost();
        }

        void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest,
            NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
        {
            if (GameManager.Instance.IsWaitingToStart())
            {
                connectionApprovalResponse.Approved = true;
                connectionApprovalResponse.CreatePlayerObject = true;
            }
            else
                connectionApprovalResponse.Approved = false;
        }

        public void StartClient()
        {
            NetworkManager.Singleton.StartClient();
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

        public int GetKitchenObjectSOIndex(SO_KitchenObject kitchenObjectSO)
        {
            return _kitchenObjectListSO.KitchenObjectList.IndexOf(kitchenObjectSO);
        }

        public SO_KitchenObject GetKitchenObjectSOFromIndex(int kitchenObjectSOIndex)
        {
            return _kitchenObjectListSO.KitchenObjectList[kitchenObjectSOIndex];
        }

        public void DestroyKitchenObject(KitchenObject kitchenObject)
        {
            DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
        }

        [ServerRpc(RequireOwnership = false)]
        void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObject_NetworkObjRef)
        {
            kitchenObject_NetworkObjRef.TryGet(out NetworkObject kitchenObject_NetworkObject);
            KitchenObject kitchenObject = kitchenObject_NetworkObject.GetComponent<KitchenObject>();

            ClearKitchenObjectHolderClientRpc(kitchenObject_NetworkObjRef);
            //only server can destroy objects
            kitchenObject.DestroySelf();
        }

        [ClientRpc]
        void ClearKitchenObjectHolderClientRpc(NetworkObjectReference kitchenObject_NetworkObjRef)
        {
            kitchenObject_NetworkObjRef.TryGet(out NetworkObject kitchenObject_NetworkObject);
            KitchenObject kitchenObject = kitchenObject_NetworkObject.GetComponent<KitchenObject>();

            kitchenObject.ClearKitchenObjectHolder();
        }
    }
}
