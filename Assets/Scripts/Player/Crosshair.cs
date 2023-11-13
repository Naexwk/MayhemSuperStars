using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Crosshair : MonoBehaviour
{

    Vector2 input_ShootDirection;
    Vector2 direction;

    [SerializeField] private float distance = 10f;
    Color myColor, tmp;

    public void OnShootDirection(InputAction.CallbackContext context){
        input_ShootDirection = context.ReadValue<Vector2>();
    }

    private void Start() {
        myColor = GetComponent<SpriteRenderer>().color;
        tmp = myColor;
        tmp.a = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent.GetComponent<PlayerInput>().devices[0].ToString() == "Keyboard:/Keyboard") {
            direction = Vector2.zero;
        } else {
            direction = input_ShootDirection;
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
