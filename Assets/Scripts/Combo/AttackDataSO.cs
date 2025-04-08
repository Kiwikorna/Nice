using Combo;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[System.Serializable]
[CreateAssetMenu(menuName = "ComboSystem/Data")]
public class AttackDataSO : ScriptableObject
{
    public AttackTypeSO attackTypeSo;
    public float bufferTime = 0.3f; // Время для комбо-окна
    public float cooldown = 0.5f; // Время между атаками
    public float animationLength; 


}

