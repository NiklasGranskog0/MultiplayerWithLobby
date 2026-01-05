using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework_TempName.ExtensionScripts;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.ScriptableObjects.SerializedDictionaries;
using Project_Assets.Scripts.Structs;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Assets.Scripts.Lobby
{
    // TODO: Create Game UI is only a temporary page for creating a lobby.
    public class CreateLobbyUI : MonoBehaviour
    {
        [Header("Lobby Settings Input Fields")]
        [SerializeField] private TMP_InputField m_lobbyNameInputField;
        [SerializeField] private TMP_InputField m_createLobbyPasswordInputField;
        
        [Header("Lobby Settings Dropdowns")]
        [SerializeField] private TMP_Dropdown m_gameModeDropdown;
        [SerializeField] private TMP_Dropdown m_maxPlayersDropdown;
        [SerializeField] private TMP_Dropdown m_mapDropdown;
        [SerializeField] private TMP_Dropdown m_visibilityDropdown;
        [SerializeField] private TMP_Dropdown m_gameSpeedDropdown;
        
        [Header("Lobby Buttons")]
        [SerializeField] private Button m_createLobbyButton;
        [SerializeField] private Button m_cancelCreateLobbyButton;
        [SerializeField] private Button m_setMapImageButton;
        
        [Header("Lobby Preview Image")]
        [SerializeField] private RawImage m_previewGameImage;
        [SerializeField] private ImagesDictionary m_gameImagesDictionary;
        
        // temp
        private string m_gameImageName; 
        
        private LobbyManager m_lobbyManager;
        private PanelSwitcher m_panelSwitcher;
        private ErrorMessageText m_errorMessage;
        private LobbyUI m_lobbyUI;
        
        
        private void Awake()
        {
            ServiceLocator.ForSceneOf(this).Register(this, ServiceLevel.Scene, gameObject.scene.name);
        }
        
        private void Start()
        {
            ServiceLocator.ForSceneOf(this).Get(out m_lobbyManager);
            ServiceLocator.ForSceneOf(this).Get(out m_panelSwitcher);
            ServiceLocator.ForSceneOf(this).Get(out m_errorMessage);
            ServiceLocator.ForSceneOf(this).Get(out m_lobbyUI);

            m_createLobbyButton.onClick.AddListener(OnCreateLobby);
            m_setMapImageButton.onClick.AddListener(OnSetMapImagePreview);
            m_visibilityDropdown.onValueChanged.AddListener(GameVisibilityChanged);
            m_cancelCreateLobbyButton.onClick.AddListener(OnCancelCreateLobby);

            // Value == index
            m_maxPlayersDropdown.value = 3; // (Set default value to 4 players) 
            m_createLobbyPasswordInputField.interactable = false;
        }
        
        private void OnCancelCreateLobby()
        {
            m_panelSwitcher.SwitchPanel(LobbyPanel.GamePanel);
        }
        
        private void OnSetMapImagePreview()
        {
            m_gameImageName = "B";
            m_previewGameImage.color = Color.white;
            m_previewGameImage.texture = m_gameImagesDictionary[m_gameImageName];
        }
        
        private void GameVisibilityChanged(int arg0)
        {
            m_createLobbyPasswordInputField.interactable = m_visibilityDropdown.value == 1;
        }
        
        private async void OnCreateLobby()
        {
            var settings = new CreateLobbySettings
            {
                IsLocked = false,
                IsPrivate = m_visibilityDropdown.value == 1,
                Password = m_createLobbyPasswordInputField.text,

                GameImage = (m_previewGameImage, m_gameImageName, DataObject.VisibilityOptions.Public),
                GameMode = ((GameMode)m_gameModeDropdown.value, DataObject.VisibilityOptions.Public),
                GameMap = ((Map)m_mapDropdown.value, DataObject.VisibilityOptions.Public),
                MaxPlayers = (m_maxPlayersDropdown.value + 1, DataObject.VisibilityOptions.Public),
                GameName = (m_lobbyNameInputField.text, DataObject.VisibilityOptions.Public),
                GameSpeed = ((GameSpeed)m_gameSpeedDropdown.value, DataObject.VisibilityOptions.Public),
            };

            settings.SetData();

            m_lobbyUI.LobbyInfo.GameName.text = settings.GameName.name;
            m_lobbyUI.LobbyInfo.MaxPlayers.text = settings.MaxPlayers.max.ToString();
            m_lobbyUI.LobbyInfo.GameSpeed.text = settings.GameSpeed.speed.GameSpeedToString();
            m_lobbyUI.LobbyInfo.GameMode.text = settings.GameMode.mode.GameModeToString();
            m_lobbyUI.LobbyInfo.MapName.text = settings.GameMap.map.GameMapToString();
            m_lobbyUI.LobbyInfo.GameImage.color = Color.white;
            m_lobbyUI.LobbyInfo.GameImage.texture = settings.GameImage.image.texture;

            if (m_createLobbyPasswordInputField.text.Length < 8 && m_visibilityDropdown.value == 1)
            {
                m_errorMessage.ShowError("Password must be at least 8 characters long", LobbyPanel.CreatePanel);
                return;
            }
            
            var task = await m_lobbyManager.CreateLobbyAsync(settings, m_createLobbyPasswordInputField.interactable);
            PrintStatusLog(task, LobbyPanel.CreatePanel);
        }
        
        private void PrintStatusLog(StatusReport task, LobbyPanel panel)
        {
            if (!task.Success)
            {
                m_errorMessage.ShowError(task.Message, panel);
                return;
            }

            task.Log();
        }
    }
}
