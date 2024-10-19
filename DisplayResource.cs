using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DisplayResource : MonoBehaviour
{
    [SerializeField] GameObject player;
    PlayerController playerController;
    [SerializeField] Text text;
    [SerializeField] public String resourceName;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent <PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = playerController.crystals.ToString();
    }

    void ShowResource()
    {
        text.text = $"{resourceName}: {playerController.crystals}";
    }

}
