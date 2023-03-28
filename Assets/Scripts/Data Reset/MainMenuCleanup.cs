using KitchenChaos.Interactions.Multiplayer;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace KitchenChaos.DataReset
{
    public class MainMenuCleanup : MonoBehaviour
    {
        void Awake()
        {
            if (NetworkManager.Singleton != null)
                Destroy(NetworkManager.Singleton.gameObject);

            if (GameMultiplayer.Instance != null)
                Destroy(GameMultiplayer.Instance.gameObject);
        }
    }
}
