using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using KitchenChaos.Interactions.Multiplayer;
using Unity.Netcode;

namespace KitchenChaos.Multiplayer.UI
{
    public class ConnectionResponseUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _messageText;
        [SerializeField] Button _closeButton;

        void Awake()
        {
            _closeButton.onClick.AddListener(Hide);
        }

        void Start()
        {
            GameMultiplayer.Instance.OnFailedToJoinGame += GameMultiplayer_OnFailedToJoinGame;
            Hide();
        }

        void GameMultiplayer_OnFailedToJoinGame()
        {
            Show();

            _messageText.text = NetworkManager.Singleton.DisconnectReason;

            if (_messageText.text == "")
                _messageText.text = "Failed to connect";
        }

        void Show()
        {
            gameObject.SetActive(true);
        }

        void Hide()
        {
            gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            GameMultiplayer.Instance.OnFailedToJoinGame -= GameMultiplayer_OnFailedToJoinGame;
        }
    }
}
