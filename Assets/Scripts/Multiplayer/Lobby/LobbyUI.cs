using KitchenChaos.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenChaos.Multiplayer.UI
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] Button _mainMenuButton;
        [SerializeField] Button _createLobbyButton;
        [SerializeField] Button _quickJoinButton;

        void Awake()
        {
            _mainMenuButton.onClick.AddListener(() =>
            {
                Loader.LoadScene(Loader.Scene.MainMenuScene);
            });

            _createLobbyButton.onClick.AddListener(() =>
            {
                KitchenGameLobby.Instance.CreateLobby("MyLobby", false);
            });

            _quickJoinButton.onClick.AddListener(() =>
            {
                KitchenGameLobby.Instance.QuickJoin();
            });
        }





    }
}
