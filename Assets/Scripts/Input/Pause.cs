using UnityEngine;
using KitchenChaos.Core;
using System;

namespace KitchenChaos.PlayerInput
{
    public class Pause : MonoBehaviour
    {
        public event Action OnPauseButtonPressed;

        [SerializeField] GameInput _input;
        bool _paused;
        public bool Paused => _paused;

        void Start()
        {
            _input.OnPauseAction += _input_OnPauseAction;
        }

        void _input_OnPauseAction()
        {
            TogglePauseGame();
        }

        internal void TogglePauseGame()
        {
            if (GameManager.Instance.IsGameOver()) return;
        
            _paused = !_paused;

            OnPauseButtonPressed?.Invoke();

            if (_paused)
                Time.timeScale = 0f;
            else
                Time.timeScale = 1f;
        }

        void OnDestroy()
        {
            _input.OnPauseAction -= _input_OnPauseAction;
        }
    }
}
