using Unity.Netcode;
using UnityEngine;

namespace Project_Assets.Scripts.Game
{
    public class SpawnPlayerTest : MonoBehaviour
    {
        public GameObject player;
        
        private void Start()
        {
            NetworkManager.Instantiate(player, null);
        }
    }
}
