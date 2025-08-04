using TMPro;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Assets.Scripts.Lobby
{
    public class ChatListItem : MonoBehaviour
    {
        [SerializeField] private ScrollRect textScrollRect;
        [SerializeField] private TMP_Text chatMessage;
        [SerializeField] private Button enterButton;

        public void SetTextMessage(VivoxMessage message)
        {
            
        }
    }
}
