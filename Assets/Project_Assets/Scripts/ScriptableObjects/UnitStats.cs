using Project_Assets.Scripts.Enums;
using UnityEngine;

namespace Project_Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "UnitStats", menuName = "Scriptable Objects/UnitStats")]
    public class UnitStats : ScriptableObject
    {
        public ImageToLoad UnitImageToLoad;
        public string UnitName;
        
        public float Health;
        public float Armor;
        public float MovementSpeed;

        public float Damage;
        public float AttackRange;
        public float AttackCooldown;
        
        // public bool IsFlying;
    }
}