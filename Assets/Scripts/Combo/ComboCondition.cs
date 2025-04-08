using UnityEngine;

public abstract class ComboCondition : ScriptableObject
{
    public abstract bool CheckCondition(PlayerController player, ComboSystem system);
}
