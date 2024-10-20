using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager: MonoBehaviour
{
    [SerializeField] private Texture2D defaultCursorTexture;
    [SerializeField] private Texture2D enemyHoverCursor;
    public LayerMask enemyLayer;

    private void Start()
    {
        Cursor.SetCursor(defaultCursorTexture, new Vector2(10, 10), CursorMode.Auto);
    }

    private void Update()
    {
        HoveringOverEnemy();   
    }

    void HoveringOverEnemy()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, enemyLayer)){
            Cursor.SetCursor(enemyHoverCursor, new Vector2(0, 0), CursorMode.Auto);
        }

        else
        {
            Cursor.SetCursor(defaultCursorTexture, new Vector2(10, 10), CursorMode.Auto);
        }
    }
    }


