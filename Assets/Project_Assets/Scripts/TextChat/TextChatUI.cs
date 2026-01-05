using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.Lobby;
using Project_Assets.Scripts.ScriptableObjects;
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
        [SerializeField] private GameObject m_chatListItem;
        [SerializeField] private Transform m_chatContainer;
        [SerializeField] private TMP_InputField m_chatInputField;
        [SerializeField] private ScrollRect m_chatScrollRect;
        [SerializeField] private UIInputs m_uiInputs;

        private readonly IList<KeyValuePair<string, ChatListItem>> m_messageObjectPool =
            new List<KeyValuePair<string, ChatListItem>>();

        private DateTime? m_oldestMessage;

        private VivoxManager m_vivoxManager;
        private LobbyManager m_lobbyManager;
        private static StatusReport s_statusReport;

        private void Start()
        {
            ServiceLocator.Global.Get(out m_vivoxManager);
            ServiceLocator.ForSceneOf(this).Get(out m_lobbyManager);
            
            m_chatScrollRect.verticalScrollbar.gameObject.SetActive(false);
            m_chatScrollRect.horizontalScrollbar.gameObject.SetActive(false);
            m_chatScrollRect.enabled = false;
            
            VivoxService.Instance.ChannelJoined += OnChannelJoined;
            VivoxService.Instance.ChannelMessageReceived += OnChannelMessageReceived;

            m_lobbyManager.OnLeftTextChannel += OnLeftTextChannel;
            m_lobbyManager.OnSendSystemMessage += SystemSendMessage;
            
            m_chatScrollRect.onValueChanged.AddListener(ScrollRectChange);
            
            m_uiInputs.OnReturnKeyEvent += EnterKeyOnTextField;
        }

        private void OnLeftTextChannel(string obj)
        {
            ClearMessagePool();
            m_oldestMessage = null;
        }

        private void OnEnable() => ClearTextInput();

        private void OnDisable()
        {
            if (m_messageObjectPool.Count > 0) ClearMessagePool();
            m_oldestMessage = null;
            
            VivoxService.Instance.ChannelJoined -= OnChannelJoined;
            VivoxService.Instance.ChannelMessageReceived -= OnChannelMessageReceived;
        }

        private async Task<StatusReport> GetChatHistory(bool scrollToBottom = false)
        {
            try
            {
                var chatHistoryOptions = new ChatHistoryQueryOptions
                {
                    TimeEnd = m_oldestMessage
                };

                var historyMessages =
                    await VivoxService.Instance.GetChannelTextMessageHistoryAsync(m_vivoxManager.CurrentChannelName, 10,
                        chatHistoryOptions);

                var reversedMessages = historyMessages.Reverse();

                foreach (var historyMessage in reversedMessages)
                {
                    AddMessageToChat(historyMessage, true, scrollToBottom);
                }

                m_oldestMessage = historyMessages.FirstOrDefault()?.ReceivedTime;
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
            foreach (var keyValuePair in m_messageObjectPool)
            {
                if (keyValuePair.Value != null)
                    Destroy(keyValuePair.Value.gameObject);
            }
            
            m_messageObjectPool.Clear();
        }

        private void ClearTextInput()
        {
            m_chatInputField.text = string.Empty;
            m_chatInputField.Select();
            m_chatInputField.ActivateInputField();
        }

        private IEnumerator ScrollToBottom()
        {
            yield return new WaitForEndOfFrame();
            m_chatScrollRect.verticalNormalizedPosition = 0;
            yield return null;
        }

        private async void ScrollRectChange(Vector2 vector)
        {
            if (m_chatScrollRect.verticalNormalizedPosition >= 0.95f)
            {
                m_chatScrollRect.normalizedPosition = new Vector2(0, 0.8f);
                var getMessages = await GetChatHistory(false);
                getMessages.Log();
            }
        }

        private void EnterKeyOnTextField()
        {
            // if (!Input.GetKeyDown(KeyCode.Return)) return;
            
            SendMessage();
        }

        private async void SendMessage()
        {
            if (string.IsNullOrEmpty(m_chatInputField.text)) return;
            
            await VivoxService.Instance.SendChannelTextMessageAsync(m_vivoxManager.CurrentChannelName, m_chatInputField.text);
            ClearTextInput();
        }

        private async void SystemSendMessage(string message)
        {
            await VivoxService.Instance.SendChannelTextMessageAsync(m_vivoxManager.CurrentChannelName, message);
        }

        private async void OnChannelJoined(string s)
        {
            var getMessages = await GetChatHistory(true);
            getMessages.Log();
        }
        
        private void OnChannelMessageReceived(VivoxMessage message)
        {
            AddMessageToChat(message, false, true);
        }

        private void AddMessageToChat(VivoxMessage message, bool isHistory = false, bool scrollToBottom = false)
        {
            var newMessageObj = Instantiate(m_chatListItem, m_chatContainer).GetComponent<ChatListItem>();

            if (isHistory)
            {
                m_messageObjectPool.Insert(0, new KeyValuePair<string, ChatListItem>(message.MessageId, newMessageObj));
                newMessageObj.transform.SetSiblingIndex(0);
            }
            else
            {
                m_messageObjectPool.Add(new KeyValuePair<string, ChatListItem>(message.MessageId, newMessageObj));
            }
            
            newMessageObj.SetMessage(message);
            
            if (scrollToBottom) StartCoroutine(ScrollToBottom());
        }
    }
}