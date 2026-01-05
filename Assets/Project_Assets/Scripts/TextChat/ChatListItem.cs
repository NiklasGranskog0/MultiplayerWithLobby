using TMPro;
using Unity.Services.Vivox;
using UnityEngine;

namespace Project_Assets.Scripts.TextChat
{
    public class ChatListItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_chatMessage;

        public void SetMessage(VivoxMessage message)
        {
            m_chatMessage.text = $"[{message.SenderDisplayName}] {message.MessageText}";
            gameObject.SetActive(true);
        }
    }
}
