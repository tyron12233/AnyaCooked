using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

namespace KitchenChaos.Multiplayer.UI
{
    public class TestingNetcodeUI : MonoBehaviour
    {
        [SerializeField] Button _startHostButton;
        [SerializeField] Button _startClientButton;

        void Awake()
        {
            _startHostButton.onClick.AddListener(() =>
            {
                Debug.Log("Host");
                NetworkManager.Singleton.StartHost();
                Hide();
            });

            _startClientButton.onClick.AddListener(() =>
            {
                Debug.Log("Client");
                NetworkManager.Singleton.StartClient();
                Hide();
            });
        }

        void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
