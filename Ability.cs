using UnityEngine;

public class Ability : MonoBehaviour
{
    // You can add more properties like cooldowns, mana cost, etc.
    public string abilityName;
    public float cooldownTime;
    public float abilityPower;

    public virtual void Activate()
    {
        // Placeholder for ability logic
        Debug.Log($"{abilityName} activated!");
    }
}
