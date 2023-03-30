using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using KitchenChaos.Core;

namespace KitchenChaos.Multiplayer.UI
{
    public class CharacterSelectUI : MonoBehaviour
    {
        [SerializeField] Button _mainMenuButton;
        [SerializeField] Button _readyButton;
        [SerializeField] CharacterSelectReady _characterSelectReady;

        private void Awake()
        {
            _mainMenuButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.Shutdown();
                Loader.LoadScene(Loader.Scene.MainMenuScene);
            });

            _readyButton.onClick.AddListener(() =>
            {
                _characterSelectReady.SetPlayerReady();
            });
        }
    }
}
