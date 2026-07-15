using System;
using System.Collections;
using Project_Assets.Scripts.Framework.UnityServiceLocator;
using Project_Assets.Scripts.ScriptableObjects;
using TMPro;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project_Assets.Scripts.TextChat
{
    public class GameTextChatUI : MonoBehaviour
    {
        [SerializeField] private GameObject m_gameChatObject;
        [SerializeField] private EventSystem m_eventSystem;
        [SerializeField] private GameObject m_chatListItem;
        [SerializeField] private Transform m_chatContainer;
        [SerializeField] private TMP_InputField m_chatInputField;
        [SerializeField] private ScrollRect m_chatScrollRect;
        [SerializeField] private UIInputs m_uiInputs;

        private VivoxManager m_vivoxManager;

        private void Start()
        {
            ServiceLocator.Global.Get(out m_vivoxManager);
            m_chatScrollRect.onValueChanged.AddListener(ScrollRectChange);

            VivoxService.Instance.ChannelMessageReceived += OnChannelMessageReceived;
            VivoxService.Instance.ChannelLeft += OnChannelLeft;

            m_uiInputs.OnEnterKeyEvent += HandleChatInput;
        }

        private void OnDisable()
        {
            OnChannelLeft(string.Empty);
        }

        private async void OnChannelLeft(string channelName)
        {
            try
            {
                await VivoxService.Instance.LeaveAllChannelsAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        // TODO: Wrap text message to a new line if it exceeds the max character limit
        // TODO: Close chat window on enter if chat input field is empty, close on ESC key
        private void HandleChatInput()
        {
            m_chatInputField.gameObject.SetActive(true);
            m_eventSystem.SetSelectedGameObject(m_chatInputField.gameObject, null);
            m_chatInputField.ActivateInputField();
            SendMessage();
        }

        private void ScrollRectChange(Vector2 arg0)
        {
            if (m_chatScrollRect.verticalNormalizedPosition >= 0.95f)
            {
                m_chatScrollRect.normalizedPosition = new Vector2(0, 0.8f);
            }
        }

        private void OnChannelMessageReceived(VivoxMessage message)
        {
            AddMessageToChat(message, true);
        }

        private async void SendMessage()
        {
            if (string.IsNullOrEmpty(m_chatInputField.text)) return;
            
            await VivoxService.Instance.SendChannelTextMessageAsync(m_vivoxManager.CurrentChannelName,
                m_chatInputField.text);
            ClearTextInput();
        }

        private void AddMessageToChat(VivoxMessage message, bool scrollToBottom = false)
        {
            var newMessageObj = Instantiate(m_chatListItem, m_chatContainer).GetComponent<ChatListItem>();

            newMessageObj.SetMessage(message);

            if (scrollToBottom) StartCoroutine(ScrollToBottom());
        }

        private IEnumerator ScrollToBottom()
        {
            yield return new WaitForEndOfFrame();
            m_chatScrollRect.verticalNormalizedPosition = 0;
            yield return null;
        }

        private void ClearTextInput()
        {
            m_chatInputField.text = string.Empty;
            m_chatInputField.gameObject.SetActive(false);
        }
    }
}