using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{

    Vector2 input_ShootDirection;
    Vector2 direction;
    [SerializeField] private PlayerController player;
    [SerializeField] private PlayerInput playerInput;

    [SerializeField] private float distance = 10f;
    [SerializeField] private Image crosshairUI;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Camera myCamera;

    Vector3 cameraRelative;

    Color myColor, tmp;

    private void Start() {
        if (crosshairUI != null) {
            myColor = crosshairUI.color;
            tmp = myColor;
            tmp.a = 0f;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.enableControl || playerInput.devices[0].ToString() == "Keyboard:/Keyboard" || playerInput.devices[0].ToString() == "Mouse:/Mouse") {
            direction = Vector2.zero;
        } else {
            direction = player.input_ShootDirection;
            direction.Normalize();
            direction = direction * distance;
            transform.localPosition = direction;
        }

        if (crosshairUI != null) {
            crosshairUI.gameObject.transform.position = myCamera.WorldToScreenPoint(transform.parent.transform.position + (transform.localPosition));
        }
        
        if (direction == Vector2.zero) {
            GetComponent<SpriteRenderer>().color = tmp;
            crosshairUI.color = tmp;
        } else {
            crosshairUI.color = myColor;
            GetComponent<SpriteRenderer>().color = tmp;
            //GetComponent<SpriteRenderer>().color = myColor;
        }
    }
}
