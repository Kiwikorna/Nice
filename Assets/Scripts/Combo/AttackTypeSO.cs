using UnityEngine;

namespace Combo
{
    [CreateAssetMenu(menuName = "ComboSystem/AttackType")]
    public class AttackTypeSO : ScriptableObject
    {
        public string attackName;
        public AnimationClip animationClip;
        public float baseDamage = 10f;
    }
}
