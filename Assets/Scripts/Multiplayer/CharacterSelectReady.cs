using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using KitchenChaos.Core;

namespace KitchenChaos.Multiplayer
{
    public class CharacterSelectReady : NetworkBehaviour
    {
        Dictionary<ulong, bool> _playerReadyDictionary;

        void Awake()
        {
            _playerReadyDictionary = new Dictionary<ulong, bool>();
        }

        public void SetPlayerReady()
        {
            print("Player ready!");
            SetPlayerReadyServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
        {
            _playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

            bool allClientsReady = true;

            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (!_playerReadyDictionary.ContainsKey(clientId) || !_playerReadyDictionary[clientId])
                {
                    allClientsReady = false;
                    break;
                }
            }

            if (allClientsReady)
                Loader.LoadSceneNetwork(Loader.Scene.GameScene);
        }
    }
}
