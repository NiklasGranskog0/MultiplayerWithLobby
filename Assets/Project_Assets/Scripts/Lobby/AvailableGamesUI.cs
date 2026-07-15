using System.Collections.Generic;
using System.Threading.Tasks;
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Events;
using Project_Assets.Scripts.Framework.ExtensionScripts;
using Project_Assets.Scripts.Framework.UnityServiceLocator;
using Project_Assets.Scripts.Structs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Assets.Scripts.Lobby
{
    public class AvailableGamesUI : MonoBehaviour
    {
        [SerializeField] private GameObject m_gameListItemPrefab;
        [SerializeField] private Transform m_gameListItemContainer;
        
        [Header("Text Input Fields")]
        [SerializeField] private TMP_InputField m_gameCodeInputField;
        [SerializeField] private TMP_InputField m_gamePasswordInputField;
        [SerializeField] private TMP_Text m_gameNameText;

        [Header("Buttons")]
        [SerializeField] private Button m_createGameButton;
        [SerializeField] private Button m_joinGameButton;
        [SerializeField] private Button m_refreshGamesButton;

        // TODO: This is probably gonna be the logout button
        [SerializeField] private Button m_quitButton;

        [Header("Game Info")]
        [SerializeField] private LobbyInfo m_gameInfo;
        public TMP_Text GameNameText;
        
        public LobbyInfo GameInfo => m_gameInfo;
        public TMP_InputField GameCodeInputField => m_gameCodeInputField;

        private LobbyManager m_lobbyManager;
        private ErrorMessageText m_errorMessage;
        private PanelSwitcher m_panelSwitcher;
        
        public Unity.Services.Lobbies.Models.Lobby CurrentSelectedLobby { get; set; }
        
        private void Awake()
        {
            ServiceLocator.ForSceneOf(this).Register(this, ServiceLevel.Scene, gameObject.scene.name);
        }
        
        private void Start()
        {
            ServiceLocator.ForSceneOf(this).Get(out m_lobbyManager);
            ServiceLocator.ForSceneOf(this).Get(out m_errorMessage);
            ServiceLocator.ForSceneOf(this).Get(out m_panelSwitcher);
            
            m_lobbyManager.LobbyListChanged += OnLobbyListChanged;
            
            m_createGameButton.onClick.AddListener(OnCreateGameButtonClick);
            m_joinGameButton.onClick.AddListener(JoinSelectedGame);
            m_refreshGamesButton.onClick.AddListener(OnRefreshGameList);
            m_quitButton.onClick.AddListener(OnQuitButtonClick);
            
            OnRefreshGameList();
        }

        private void OnQuitButtonClick()
        {
            Application.Quit();
        }

        private void OnCreateGameButtonClick()
        {
            m_panelSwitcher.SwitchPanel(LobbyPanel.CreatePanel);
        }
        
        // TODO: When OnRefreshGameList is called, the LobbyListChanged event is fired, which makes this redundant
        // When querying for lobbies, populate the list with them
        private void OnLobbyListChanged(LobbyListChangedEventArgs obj)
        {
            PopulateLobbyList(obj.Lobbies);
            Debug.Log("Populate Lobby List (LobbyListChanged)".Color("cyan"));
        }
        
        // When refreshing, get all the active lobbies and populate the game list
        private async void OnRefreshGameList()
        {
            var task = await m_lobbyManager.GetAllActiveLobbiesAsync();
            PopulateLobbyList(task.Lobbies);
            PrintStatusLog(task.Status, LobbyPanel.GamePanel);
        }
        
        // Clear the game list and populate it with new lobbies if there are any
        private void PopulateLobbyList(List<Unity.Services.Lobbies.Models.Lobby> lobbies)
        {
            m_gameListItemContainer.ClearContainer();
            
            if (lobbies == null || lobbies.Count == 0)
            {
                Debug.Log("No lobbies found".Color("red"));
                return;
            }

            foreach (var lobby in lobbies)
            {
                var entry = Instantiate(m_gameListItemPrefab, m_gameListItemContainer).GetComponent<GameListItem>();
                entry.UpdateLobby(lobby);
                entry.gameObject.SetActive(true);
            }
        }
        
        // Join the selected game
        private async void JoinSelectedGame()
        {
            bool joinedByCode = false;
        
            if (CurrentSelectedLobby == null)
            {
                // TODO: what is the point of this bool
                joinedByCode = await TryJoinByCode();
            }
        
            // If we could not join by code, try joining by lobby id
            if (!string.IsNullOrEmpty(CurrentSelectedLobby?.Id) && !joinedByCode)
            {
                var task = await m_lobbyManager.JoinLobbyByIdAsync(CurrentSelectedLobby?.Id, m_gamePasswordInputField.text);
                PrintStatusLog(task, LobbyPanel.GamePanel);
            }
        }
        
        // Try joining by code
        private async Task<bool> TryJoinByCode()
        {
            if (!string.IsNullOrWhiteSpace(m_gameCodeInputField.text))
            {
                var task =
                    await m_lobbyManager.JoinLobbyByCodeAsync(m_gameCodeInputField.text, m_gamePasswordInputField.text);
                PrintStatusLog(task, LobbyPanel.GamePanel);
            
                return task.Success;
            }
        
            return false;
        }
        
        // Print the status log
        private void PrintStatusLog(StatusReport task, LobbyPanel panel)
        {
            // if (!task.Success)
            // {
            //     m_errorMessage.ShowError(task.Message, panel);
            //     return;
            // }

            task.Log();
        }
    }
}
