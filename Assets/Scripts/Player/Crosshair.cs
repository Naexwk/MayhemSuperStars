using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    float xInput;
    float yInput;
    Vector2 direction;
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
        xInput = Input.GetAxisRaw("Horizontal Aim");
        yInput = Input.GetAxisRaw("Vertical Aim");
        if (Mathf.Abs(xInput) < 0.2f && Mathf.Abs(yInput) < 0.2f) {
            GetComponent<SpriteRenderer>().color = tmp;
        } else {
            GetComponent<SpriteRenderer>().color = myColor;
        }
        direction = new Vector2(xInput, yInput);
        direction.Normalize();
        direction = direction * distance;
        transform.localPosition = direction;
    }
}
