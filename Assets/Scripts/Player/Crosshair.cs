using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Crosshair : MonoBehaviour
{

    Vector2 input_ShootDirection;
    Vector2 direction;
    [SerializeField] private PlayerController player;
    [SerializeField] private PlayerInput playerInput;

    [SerializeField] private float distance = 10f;
    Color myColor, tmp;

    private void Start() {
        myColor = GetComponent<SpriteRenderer>().color;
        tmp = myColor;
        tmp.a = 0f;
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
        
        if (direction == Vector2.zero) {
            GetComponent<SpriteRenderer>().color = tmp;
        } else {
            GetComponent<SpriteRenderer>().color = myColor;
        }
    }
}
