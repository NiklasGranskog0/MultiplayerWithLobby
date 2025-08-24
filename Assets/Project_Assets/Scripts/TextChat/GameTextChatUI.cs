using System.Collections;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.Lobby;
using TMPro;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project_Assets.Scripts.TextChat
{
    public class GameTextChatUI : MonoBehaviour
    {
        [SerializeField] private GameObject chatListItem;
        [SerializeField] private Transform chatContainer;
        [SerializeField] private TMP_InputField chatInputField;
        [SerializeField] private ScrollRect chatScrollRect;
        
        private VivoxManager m_VivoxManager;
        
        private void Start()
        {
            ServiceLocator.Global.Get(out m_VivoxManager);
            chatScrollRect.onValueChanged.AddListener(ScrollRectChange);
            
            VivoxService.Instance.ChannelMessageReceived += OnChannelMessageReceived;
            VivoxService.Instance.ChannelLeft += OnChannelLeft;
        }

        private void OnDisable()
        {
            OnChannelLeft(null);
        }

        private async void OnChannelLeft(string obj)
        {
            await VivoxService.Instance.LeaveAllChannelsAsync();
        }

        // TODO: Wrap sent message so text doesn't get smaller
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                chatInputField.gameObject.SetActive(true);
                EventSystem.current.SetSelectedGameObject(chatInputField.gameObject, null);
                chatInputField.ActivateInputField();
                
                if (string.IsNullOrEmpty(chatInputField.text)) return;
                SendMessage();
            }
        }

        private void ScrollRectChange(Vector2 arg0)
        {
            if (chatScrollRect.verticalNormalizedPosition >= 0.95f)
            {
                chatScrollRect.normalizedPosition = new Vector2(0, 0.8f);
            }
        }
        
        private void OnChannelMessageReceived(VivoxMessage message)
        {
            AddMessageToChat(message, true);
        }

        private async void SendMessage()
        {
            if (string.IsNullOrEmpty(chatInputField.text)) return;
            
            await VivoxService.Instance.SendChannelTextMessageAsync(m_VivoxManager.CurrentChannelName, chatInputField.text);
            ClearTextInput();
        }
        
        private void AddMessageToChat(VivoxMessage message, bool scrollToBottom = false)
        {
            var newMessageObj = Instantiate(chatListItem, chatContainer).GetComponent<ChatListItem>();

            newMessageObj.SetMessage(message);
            
            if (scrollToBottom) StartCoroutine(ScrollToBottom());
        }
        
        private IEnumerator ScrollToBottom()
        {
            yield return new WaitForEndOfFrame();
            chatScrollRect.verticalNormalizedPosition = 0;
            yield return null;
        }

        private void ClearTextInput()
        {
            chatInputField.text = string.Empty;
            chatInputField.gameObject.SetActive(false);
        }
    }
}
