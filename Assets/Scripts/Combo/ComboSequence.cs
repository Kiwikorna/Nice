using Combo;
using UnityEngine;

[CreateAssetMenu(menuName = "Combo/Combo Sequence")]
public class ComboSequence : ScriptableObject
{
    public ComboChain[] combos;
}

[System.Serializable]
public class ComboChain
{
    public string name;
    public string[] attackDateIds; // Используем идентификаторы вместо прямых ссылок
    public ComboChain[] possibleBranches;
    public ComboCondition branchCondition;
    public float damageMultiplier;
    public CharacterState requiredState;
}
