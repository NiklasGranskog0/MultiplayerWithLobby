using Unity.Netcode;
using UnityEngine;

namespace Project_Assets
{
    [RequireComponent(typeof(NetworkObject))]
    public class TestScript : NetworkBehaviour
    {
        private void Start()
        {
            Debug.Log("TestScript Start", this);
        }
    }
}
