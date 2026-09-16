using System.Collections.Generic;
using Project_Assets.Scripts.UtilityExtensions.Singletons;
using UnityEngine;

namespace Project_Assets.Scripts.Game
{
    public class SpawnPoints : Singleton<SpawnPoints>
    {
        [SerializeField] private Transform[] m_spawnPoints;
        
        public IReadOnlyList<Transform> Points => m_spawnPoints;
    }
}
