using KitchenChaos.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;


namespace KitchenChaos.Multiplayer
{
    public class KitchenGameLobby : MonoBehaviour
    {
        public static KitchenGameLobby Instance { get; private set; }

        Lobby _joinedLobby;


        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAuthentication();
        }

        async void InitializeAuthentication()
        {
            if (UnityServices.State == ServicesInitializationState.Initialized) return;

            // init with a different profile every time (for testing on the same unit)
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile("Player" + Random.Range(1, 100).ToString());

            await UnityServices.InitializeAsync(initializationOptions);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        public async void CreateLobby(string lobbyName, bool isPrivate)
        {
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
            }
        }

        public async void QuickJoin()
        {
            try
            {
                _joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

                GameMultiplayer.Instance.StartClient();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }




    }
}
