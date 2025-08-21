using System.Collections.Generic;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Project_Assets.Scripts.Lobby
{
    public class HostDictionary : MonoBehaviour
    {
        public Dictionary<Player, bool> ClientsAndHost { get; private set; }

        private void Awake()
        {
            ServiceLocator.Global.Register(this, ServiceLevel.Global);
            ClientsAndHost = new Dictionary<Player, bool>();
        }
    }
}
