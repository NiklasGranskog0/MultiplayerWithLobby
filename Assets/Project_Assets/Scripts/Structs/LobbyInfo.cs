using System;
using TMPro;
using UnityEngine.UI;

namespace Project_Assets.Scripts.Structs
{
    [Serializable]
    public struct LobbyInfo
    {
        public TMP_Text GameName;
        public TMP_Text GameMode;
        public TMP_Text GameSpeed;
        public TMP_Text MaxPlayers;
        public TMP_Text MapName;

        public RawImage GameImage;
    }
}
