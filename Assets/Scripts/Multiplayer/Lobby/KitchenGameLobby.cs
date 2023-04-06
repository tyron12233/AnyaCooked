using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using KitchenChaos.Core;


namespace KitchenChaos.Multiplayer
{
    public class KitchenGameLobby : MonoBehaviour
    {
        public static KitchenGameLobby Instance { get; private set; }

        public event Action OnCreateLobbyStarted;
        public event Action OnCreateLobbyFailed;
        public event Action OnJoinStarted;
        public event Action OnJoinFailed;
        public event Action OnQuickJoinFailed;
        public event Action<List<Lobby>> OnLobbyListChanged;

        [SerializeField] float _heartBeatTimerMax = 15f;
        [SerializeField] float _refreshLobbiesTimerMax = 3f;

        Lobby _joinedLobby;
        float _heartBeatTimer;
        float _refreshLobbiesTimer;

        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAuthentication();
        }

        void Update()
        {
            HandleHeartbeat();
            PeriodicallyRefreshLobbyList();
        }

        void HandleHeartbeat()
        {
            if (!IsLobbyHost()) return;

            _heartBeatTimer -= Time.deltaTime;

            if (_heartBeatTimer <= 0)
            {
                _heartBeatTimer = _heartBeatTimerMax;
                LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
            }
        }

        void PeriodicallyRefreshLobbyList()
        {
            // if this throws error, wrap in if with &&
            if (_joinedLobby != null || !AuthenticationService.Instance.IsSignedIn) 
                return;

            _refreshLobbiesTimer -= Time.deltaTime;

            if (_refreshLobbiesTimer <= 0)
            {
                _refreshLobbiesTimer = _refreshLobbiesTimerMax;
                ListLobbies();
            }
        }

        bool IsLobbyHost()
        {
            return _joinedLobby != null && _joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
        }

        async void ListLobbies()
        {
            try
            {
                QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
                {
                    Filters = new List<QueryFilter>
                    {
                        new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                    }
                };

                QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
                OnLobbyListChanged?.Invoke(queryResponse.Results);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        async void InitializeAuthentication()
        {
            if (UnityServices.State == ServicesInitializationState.Initialized) return;

            // init with a different profile every time (for testing on the same unit)
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile("Player" + UnityEngine.Random.Range(1, 100).ToString());

            await UnityServices.InitializeAsync(initializationOptions);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        public async void CreateLobby(string lobbyName, bool isPrivate)
        {
            OnCreateLobbyStarted?.Invoke();

            try
            {
                _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, GameMultiplayer.MAX_NUMBER_PLAYERS, new CreateLobbyOptions
                {
                    IsPrivate = isPrivate
                });

                GameMultiplayer.Instance.StartHost();
                Loader.LoadSceneNetwork(Loader.Scene.CharacterSelectScene);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                OnCreateLobbyFailed?.Invoke();
            }
        }

        public async void QuickJoin()
        {
            OnJoinStarted?.Invoke();

            try
            {
                _joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

                GameMultiplayer.Instance.StartClient();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                OnQuickJoinFailed?.Invoke();
            }
        }

        public async void JoinWithCode(string lobbyCode)
        {
            OnJoinStarted?.Invoke();

            try
            {
                _joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

                GameMultiplayer.Instance.StartClient();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                OnJoinFailed?.Invoke();
            }
        }

        //public lobbies cannot be joined by code!
        public async void JoinWithId(string lobbyId)
        {
            OnJoinStarted?.Invoke();

            try
            {
                _joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

                GameMultiplayer.Instance.StartClient();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                OnJoinFailed?.Invoke();
            }
        }

        public Lobby GetLobby()
        {
            return _joinedLobby;
        }

        public async void DeleteLobby()
        {
            if (_joinedLobby == null) return;

            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(_joinedLobby.Id);
                _joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }

        }

        public async void LeaveLobby()
        {
            if (_joinedLobby == null) return;

            try
            {
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                _joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async void KickPlayer(string playerId)
        {
            if (!IsLobbyHost()) return;

            try
            {
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, playerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

    }
}
