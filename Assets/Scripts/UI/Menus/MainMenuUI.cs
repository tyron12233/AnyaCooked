using UnityEngine;
using UnityEngine.UI;

namespace KitchenChaos.Core
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] Button _playButton;
        [SerializeField] Button _quitButton;

        void Awake()
        {
            Time.timeScale = 1f;
            SetupButtonListeners();
        }

        void Start()
        {
            _playButton.Select();
        }

        void SetupButtonListeners()
        {
            _playButton.onClick.AddListener(() =>
            {
                Loader.LoadScene(Loader.Scene.GameScene);
            });

            _quitButton.onClick.AddListener(() =>
            {
                Application.Quit();
            });
        }
    }
}
