using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;

public class DisplayResource : MonoBehaviour
{
    [SerializeField] GameObject player;
    PlayerController playerController;
    [SerializeField] Text text;
    [SerializeField] public String resourceName;
    // Start is called before the first frame update
    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        ShowResource(resourceName);
    }

    public void ShowResource(String resourceName)
    {

       
            // Trim any extra spaces or characters
            string cleanResourceName = resourceName.Trim(':', ' ');

            // Check if the key exists before accessing it
            if (playerController.resources.ContainsKey(cleanResourceName))
            {
                int resourceValue = playerController.resources[cleanResourceName];
                text.text = $"{cleanResourceName}: {resourceValue}";
            }
            else
            {
                Debug.LogError($"Resource '{cleanResourceName}' not found in the dictionary.");
            
        }
    }

}
