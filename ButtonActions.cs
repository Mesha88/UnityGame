using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;


public class ButtonActions : MonoBehaviour
{
    [SerializeField] GameObject player;
    PlayerController playerController;
    
    private UIDocument document;

    private Button healthButton;
    private Button speedButton;
    private Button damageButton;
    private Button attackSpeedButton;
    private Button AtackRangeButton;
    // Start is called before the first frame update
    void Awake()
    {
        document = GetComponent<UIDocument>();

        playerController = player.GetComponent<PlayerController>();


        healthButton = document.rootVisualElement.Q("UpgradeHealthButton") as Button;
        healthButton.RegisterCallback<ClickEvent>(OnUpgradeHealthClicked);

        speedButton = document.rootVisualElement.Q("UpgradeSpeedButton") as Button;
        speedButton.RegisterCallback<ClickEvent>(OnUpgradeSpeedClicked);

        damageButton = document.rootVisualElement.Q("UpgradeDamageButton") as Button;
        damageButton.RegisterCallback<ClickEvent>(OnUpgradeDamageClicked);

        attackSpeedButton = document.rootVisualElement.Q("UpgradeAttackSpeedButton") as Button;
        attackSpeedButton.RegisterCallback<ClickEvent>(OnUpgradeAttackSpeedClicked);

        AtackRangeButton = document.rootVisualElement.Q("UpgradeAttackRangeButton") as Button;
        AtackRangeButton.RegisterCallback<ClickEvent>(OnUpgradeAttackRangeClicked);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnUpgradeHealthClicked(ClickEvent evt){
        playerController.maxHealth += 5;
        playerController.health += 5;
    }

    private void OnUpgradeSpeedClicked(ClickEvent evt)
    {
        playerController.movementSpeed += 2;
        playerController.agent.speed = playerController.movementSpeed;
        
    }

    private void OnUpgradeDamageClicked(ClickEvent evt)
    {
        playerController.damage += 10;
    }

    private void OnUpgradeAttackSpeedClicked(ClickEvent evt)
    {

        playerController.attackCooldown -= 0.1f;
    }

    private void OnUpgradeAttackRangeClicked(ClickEvent evt)
    {
        playerController.attackRange += 2;
        playerController.visionRange += 2;
       
    }

    private void OnDisableHealthUpgrade()
    {
        healthButton.UnregisterCallback < ClickEvent> (OnUpgradeHealthClicked);
    }

    private void OnDisableSpeedUpgrade()
    {
        speedButton.UnregisterCallback<ClickEvent>(OnUpgradeSpeedClicked);
    }

    private void OnDisableDamageUpgrade()
    {
        damageButton.UnregisterCallback<ClickEvent>(OnUpgradeDamageClicked);
    }

    private void OnDisableAttackSpeedUpgrade()
    {
        attackSpeedButton.UnregisterCallback<ClickEvent>(OnUpgradeAttackSpeedClicked);
    }

    private void OnDisableAttackRangeUpgrade()
    {
        AtackRangeButton.UnregisterCallback<ClickEvent>(OnUpgradeAttackRangeClicked);
    }
}
