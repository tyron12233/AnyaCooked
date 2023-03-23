using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenChaos.PlayerInput
{
    public class GamePauseMultiplayerUI : MonoBehaviour
    {
        [SerializeField] Pause _pauseScript;

        private void Start()
        {
            _pauseScript.OnMultiplayerPauseButtonPressed += Pause_OnMultiplayerPauseButtonPressed;

            DisplayMultiplayerPauseScreen(false);
        }

        void Pause_OnMultiplayerPauseButtonPressed()
        {
            DisplayMultiplayerPauseScreen(!gameObject.activeSelf);
        }

        void DisplayMultiplayerPauseScreen(bool shouldDisplay)
        {
            gameObject.SetActive(shouldDisplay);
        }

    }
}
