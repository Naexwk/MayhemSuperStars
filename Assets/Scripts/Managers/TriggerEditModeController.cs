using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This script handle the editable ghost so it can turn red when unplaceable and detect wheter it can be placed 
public class TriggerEditModeController : MonoBehaviour
{
    private Renderer tempRend;
    private Color currentColor;
    public bool placeable = true;  

    void Start()
    {
        //Get the renderer and current color for future reference
        tempRend = GetComponent<Renderer>();
        currentColor = tempRend.material.color; 
    }

    // Called when another object enters and stays the trigger collider attached to this object
    // While the ghost editable stays inside a box collider it truns its color red and makes it unplaceable
    void OnTriggerStay2D(Collider2D other)
    {
        //Debug.Log("THE TYPE IS" + other.GetType() + "Triggered by: " + other.gameObject.name);
        if (other is BoxCollider2D )
        {
            placeable = false; 
            //Debug.Log("Triggered by: " + other.gameObject.name);
            float newRed = 255f;
            tempRend.material.color = new Color(newRed, currentColor.g, currentColor.b, currentColor.a);
        } 
    }

    // Called when another object exits the trigger collider attached to this object
    // When the editable ghost isn't on top of a collider it turns into the normal color and makes it placeable 
    void OnTriggerExit2D(Collider2D other)
    {
        placeable = true; 
        //Debug.Log("No longer triggered by: " + other.gameObject.name);
        tempRend.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, currentColor.a);
    }
}

