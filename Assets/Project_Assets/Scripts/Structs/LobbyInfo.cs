using System;
using TMPro;
using UnityEngine.UI;

namespace Project_Assets.Scripts.Structs
{
    [Serializable]
    public struct LobbyInfo
    {
        public TMP_Text gameName;
        public TMP_Text gameMode;
        public TMP_Text gameSpeed;
        public TMP_Text maxPlayers;

        public Image gameImage;
    }
}
