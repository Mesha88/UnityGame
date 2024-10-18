using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterClass : MonoBehaviour
{
    // VARIABLES
    public float health;
    public float maxHealth;
    public float mana;
    public float maxMana;
    public float movementSpeed;
    public float attackRange;
    public float attackSpeed;
    public float damage;
    public float healingRate;
    public float armor;
    public int crystals = 0;
    public int vespene = 0;
    float currentExperience;
    float neededExperience;
    int level;
    public List<VespeneUpgrade> collectedUpgrades = new List<VespeneUpgrade>();
    public Ability[] abilities = new Ability[4];
    
    // FUNCTION FOR TAKING DAMAGE
    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player is dead!");
        // Add your logic for handling player death here
    }

    // Other functions like applyVespeneUpgrade(), removeVespeneUpgrades() etc.
}