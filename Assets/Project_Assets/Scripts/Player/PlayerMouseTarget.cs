using Project_Assets.Scripts.ScriptableObjects;
using UnityEngine;

namespace Project_Assets.Scripts.Player
{
    public class PlayerMouseTarget : MonoBehaviour
    {
        [SerializeField] private Camera m_playerCamera;
        [SerializeField] private PlayerInputs m_playerInputs;
        private Vector3 m_mousePosition;
        private ulong m_playerId; // TODO: Might be useful or not
    }
}