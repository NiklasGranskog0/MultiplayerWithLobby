using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.Structs;
using TMPro;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

namespace Project_Assets.Scripts.TextChat
{
    public class TextChatUI : MonoBehaviour
    {
        [SerializeField] private GameObject chatListItem;
        [SerializeField] private Transform chatContainer;
        [SerializeField] private TMP_InputField chatInputField;
        [SerializeField] private ScrollRect chatScrollRect;

        private readonly IList<KeyValuePair<string, ChatListItem>> m_MessageObjectPool =
            new List<KeyValuePair<string, ChatListItem>>();

        private Task m_GetMessages;
        private DateTime? m_OldestMessage;

        private VivoxManager m_VivoxManager;
        private static StatusReport s_statusReport;

        private void Start()
        {
            ServiceLocator.Global.Get(out m_VivoxManager);
            
            chatScrollRect.verticalScrollbar.interactable = false;
            
            VivoxService.Instance.ChannelJoined += OnChannelJoined;
            VivoxService.Instance.ChannelMessageReceived += OnChannelMessageReceived;
            VivoxService.Instance.DirectedMessageReceived += OnDirectedMessageReceived;
            
            chatScrollRect.onValueChanged.AddListener(ScrollRectChange);
        }

        private void Update()
        {
            EnterKeyOnTextField();
        }

        private void OnEnable() => ClearTextInput();

        private void OnDisable()
        {
            if (m_MessageObjectPool.Count > 0) ClearMessagePool();
            m_OldestMessage = null;
        }

        private async Task<StatusReport> GetChatHistory(bool scrollToBottom = false)
        {
            try
            {
                var chatHistoryOptions = new ChatHistoryQueryOptions
                {
                    TimeEnd = m_OldestMessage
                };

                var historyMessages =
                    await VivoxService.Instance.GetChannelTextMessageHistoryAsync(m_VivoxManager.CurrentChannelName, 10,
                        chatHistoryOptions);

                var reversedMessages = historyMessages.Reverse();

                foreach (var historyMessage in reversedMessages)
                {
                    AddMessageToChat(historyMessage, true, scrollToBottom);
                }

                m_OldestMessage = historyMessages.FirstOrDefault()?.ReceivedTime;
                s_statusReport.MakeReport(true, "Fetched Chat History");
            }
            catch (TaskCanceledException e)
            {
                s_statusReport.MakeReport(false,
                    $"Chat history request was canceled, likely because of a logout or the data is no longer needed: {e.Message}");
            }
            catch (Exception e)
            {
                s_statusReport.MakeReport(false, $"Tried to fetch chat history and failed with error: {e.Message}");
            }

            return s_statusReport;
        }

        private void ClearMessagePool()
        {
            foreach (var keyValuePair in m_MessageObjectPool)
            {
                if (keyValuePair.Value != null)
                    Destroy(keyValuePair.Value.gameObject);
            }
            
            m_MessageObjectPool.Clear();
        }

        private void ClearTextInput()
        {
            chatInputField.text = string.Empty;
            chatInputField.Select();
            chatInputField.ActivateInputField();
        }

        private IEnumerator ScrollToBottom()
        {
            yield return new WaitForEndOfFrame();
            chatScrollRect.verticalNormalizedPosition = 0;
            // if (chatScrollRect.verticalScrollbar.value > 0f) chatScrollRect.verticalScrollbar.value = 0f;
            yield return null;
        }

        private void ScrollRectChange(Vector2 vector)
        {
            if (chatScrollRect.verticalNormalizedPosition >= 0.95f && m_GetMessages != null && (m_GetMessages.IsCompleted || m_GetMessages.IsFaulted || m_GetMessages.IsCanceled))
            {
                chatScrollRect.normalizedPosition = new Vector2(0, 0.8f);
                m_GetMessages = GetChatHistory(false);
            }
        }

        private void EnterKeyOnTextField()
        {
            if (!Input.GetKeyDown(KeyCode.Return)) return;
            
            SendMessage();
        }

        private void SendMessage()
        {
            if (string.IsNullOrEmpty(chatInputField.text)) return;
            
            VivoxService.Instance.SendChannelTextMessageAsync(m_VivoxManager.CurrentChannelName, chatInputField.text);
            ClearTextInput();
        }

        private void OnChannelJoined(string s)
        {
            m_GetMessages = GetChatHistory(true);
        }
        
        private void OnChannelMessageReceived(VivoxMessage message)
        {
            AddMessageToChat(message, false, true);
        }
        
        private void OnDirectedMessageReceived(VivoxMessage message)
        {
            AddMessageToChat(message, false, true);
        }

        private void AddMessageToChat(VivoxMessage message, bool isHistory = false, bool scrollToBottom = false)
        {
            var newMessageObj = Instantiate(chatListItem, chatContainer).GetComponent<ChatListItem>();

            if (isHistory)
            {
                m_MessageObjectPool.Insert(0, new KeyValuePair<string, ChatListItem>(message.MessageId, newMessageObj));
                newMessageObj.transform.SetSiblingIndex(0);
            }
            else
            {
                m_MessageObjectPool.Add(new KeyValuePair<string, ChatListItem>(message.MessageId, newMessageObj));
            }
            
            newMessageObj.SetMessage(message);
            
            if (scrollToBottom) StartCoroutine(ScrollToBottom());
        }
    }
}