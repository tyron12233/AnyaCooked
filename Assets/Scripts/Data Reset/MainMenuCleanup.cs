using KitchenChaos.Multiplayer;
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

            if (KitchenGameLobby.Instance != null)
                Destroy(KitchenGameLobby.Instance.gameObject);
        }

    }
}
