using KitchenChaos.PlayerInput;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenChaos.Core
{
    public class GameManager : MonoBehaviour
    {
        //use service locator, there are quite a few singletons already
        public static GameManager Instance { get; private set; }
        //take in a state parameter?
        public event Action OnStateChanged;

        [SerializeField] GameInput _gameInput;
        [SerializeField] float _countdownToStartTimer = 3f;
        [SerializeField] float _playingTimerMax = 10f;

        float _playingTimer;
        public float CountdownToStartTimer => _countdownToStartTimer;
        
        enum GameplayState
        {
            WaitingToStart,
            CountdownToStart,
            GamePlayingTime,
            GameOver,
        }

        GameplayState _state;
        Dictionary<GameplayState, Action> _updateMethods = new Dictionary<GameplayState, Action>();

        void Awake()
        {
            Instance = this;
            FillStateDictionary();    
        }

        void Start()
        {
            _gameInput.OnInteractAction += GameInput_OnInteractAction;

            // DEBUG TRIGGER GAME START AUTOMATICALLY
            _state = GameplayState.CountdownToStart;
            OnStateChanged?.Invoke();
        }

        void GameInput_OnInteractAction()
        {
            if (_state == GameplayState.WaitingToStart)
            {
                _state = GameplayState.CountdownToStart;
                OnStateChanged?.Invoke();
            }
        }

        void Update()
        {
            _updateMethods[_state].Invoke();
        }

        void FillStateDictionary()
        {
            _updateMethods[GameplayState.WaitingToStart] = UpdateWaitingToStart;
            _updateMethods[GameplayState.CountdownToStart] = UpdateCountdownToStart;
            _updateMethods[GameplayState.GamePlayingTime] = UpdateGamePlayingTime;
            _updateMethods[GameplayState.GameOver] = UpdateGameOver;
        }

        void UpdateWaitingToStart()
        {
        }

        void UpdateCountdownToStart()
        {
            _countdownToStartTimer -= Time.deltaTime;
            if (_countdownToStartTimer < 0f)
            {
                _state = GameplayState.GamePlayingTime;
                _playingTimer = _playingTimerMax;

                OnStateChanged?.Invoke();
            }
        }

        void UpdateGamePlayingTime()
        {
            _playingTimer -= Time.deltaTime;
            if (_playingTimer < 0f)
            {
                _state = GameplayState.GameOver;
                OnStateChanged?.Invoke();
            }
        }

        void UpdateGameOver()
        {
        }

        public bool IsWaitingToStart()
        {
            return _state == GameplayState.WaitingToStart;
        }

        public bool IsGamePlaying()
        {
            return _state == GameplayState.GamePlayingTime;
        }

        public bool IsCountdownActive()
        {
            return _state == GameplayState.CountdownToStart;
        }

        public bool IsGameOver()
        {
            return _state == GameplayState.GameOver;
        }

        public float GetGamePlayingTimeNormalized()
        {
            return 1 - (_playingTimer / _playingTimerMax);
        }
    }
}
