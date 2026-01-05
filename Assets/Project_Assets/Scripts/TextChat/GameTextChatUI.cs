using System.Collections;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
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

            m_uiInputs.OnReturnKeyEvent += HandleChatInput;
        }

        private void OnDisable()
        {
            OnChannelLeft(string.Empty);
        }

        private async void OnChannelLeft(string channelName)
        {
            await VivoxService.Instance.LeaveAllChannelsAsync();
        }

        // TODO: Wrap text message so text doesn't get smaller
        private void HandleChatInput()
        {
            m_chatInputField.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(m_chatInputField.gameObject, null);
            m_chatInputField.ActivateInputField();

            if (string.IsNullOrEmpty(m_chatInputField.text)) return;
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