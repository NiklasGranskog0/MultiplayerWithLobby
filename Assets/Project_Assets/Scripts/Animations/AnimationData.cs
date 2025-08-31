using System;
using Project_Assets.Scripts.Enums;
using UnityEngine;

namespace Project_Assets.Scripts.Animations
{
    [CreateAssetMenu(fileName = "New Animation Data", menuName = "Scriptable Objects/Animation Data", order = 0)]
    public class AnimationData : ScriptableObject
    {
        private int[] GetAnimationArray<T>(T animationEnum) where T : Enum
        {
            return animationEnum switch
            {
                PlayerAnim => m_PlayerAnimations,
                _ => throw new ArgumentOutOfRangeException(nameof(animationEnum), animationEnum, null)
            };
        }

        public int EnumToStateHash<T>(T animationEnum) where T : Enum
        {
            var animationArray = GetAnimationArray(animationEnum);
            var index = (int)(object)animationEnum;
            
            return animationArray[index];
        }

        private readonly int[] m_PlayerAnimations =
        {
            // String name in the Animator Controller
            Animator.StringToHash("Attack01_SwordAndShiled"),
            Animator.StringToHash("Attack02_SwordAndShiled"),
            Animator.StringToHash("Attack03_SwordAndShiled"),
            Animator.StringToHash("Attack04_SwordAndShiled"),
            Animator.StringToHash("Attack04_Spinning_SwordAndShield"),
            Animator.StringToHash("Attack04_Start_SwordAndShield"),
            Animator.StringToHash("Defend_SwordAndShield"),
            Animator.StringToHash("Defend_Hit_SwordAndShield"),
            Animator.StringToHash("Die01_Stay_SwordAndShield"),
            Animator.StringToHash("Die01_SwordAndShield"),
            Animator.StringToHash("GetHit01_SwordAndShield"),
            Animator.StringToHash("JumpAir_Double_InPlace_SwordAndShield"),
            Animator.StringToHash("JumpAir_Normal_InPlace_SwordAndShield"),
            Animator.StringToHash("JumpAir_Spin_InPlace_SwordAndShield"),
            Animator.StringToHash("JumpEnd_Normal_InPlace_SwordAndShield"),
            Animator.StringToHash("JumpStart_Normal_InPlace_SwordAndShield"),
            Animator.StringToHash("JumpFull_Normal_InPlace_SwordAndShield"),
            Animator.StringToHash("JumpFull_Normal_RM_SwordAndShield"),
            Animator.StringToHash("JumpFull_Spin_InPlace_SwordAndShield"),
            Animator.StringToHash("JumpFull_Spin_RM_SwordAndShield"),
            Animator.StringToHash("MoveBWD_Battle_InPlace_SwordAndShield"),
            Animator.StringToHash("MoveBWD_Battle_RM_SwordAndShield"),
            Animator.StringToHash("MoveFWD_Battle_InPlace_SwordAndShield"),
            Animator.StringToHash("MoveFWD_Battle_RM_SwordAndShield"),
            Animator.StringToHash("MoveFWD_Normal_InPlace_SwordAndShield"),
            Animator.StringToHash("MoveFWD_Normal_RM_SwordAndShield"),
            Animator.StringToHash("SprintFWD_Battle_InPlace_SwordAndShield"),
            Animator.StringToHash("SprintFWD_Battle_RM_SwordAndShield"),
            Animator.StringToHash("MoveLFT_Battle_InPlace_SwordAndShield"),
            Animator.StringToHash("MoveLFT_Battle_RM_SwordAndShield"),
            Animator.StringToHash("MoveRGT_Battle_InPlace_SwordAndShield"),
            Animator.StringToHash("MoveRGT_Battle_RM_SwordAndShield"),
            Animator.StringToHash("Dizzy_SwordAndShield"),
            Animator.StringToHash("GetUp_SwordAndShield"),
            Animator.StringToHash("Idle_Normal_SwordAndShield"),
            Animator.StringToHash("LevelUp_Battle_SwordAndShield"),
            Animator.StringToHash("Victory_Battle_SwordAndShield"),
            Animator.StringToHash("Idle_Battle_SwordAndShield")
        };
    }
}