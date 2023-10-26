using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controlador de colisiones de objeto colocable del modo edición
public class TriggerEditModeController : MonoBehaviour
{
    // Renderer y color del objeto
    private Renderer tempRend;
    private Color currentColor;

    public bool placeable = true;  

    // Inicializar renderer y color
    void Start()
    {
        tempRend = GetComponent<Renderer>();
        if (tempRend != null) {
            currentColor = tempRend.material.color; 
        }
    }

    // Mientras el prop a colocar se mantenga dentro del collider de un objeto colocado,
    // cambiar su color a rojo y hacerlo no colocable
    void OnTriggerStay2D(Collider2D other)
    {
        if (other is BoxCollider2D)
        {
            placeable = false; 
            float newRed = 255f;
            if (tempRend != null) {
                tempRend.material.color = new Color(newRed, currentColor.g, currentColor.b, currentColor.a);
            }
            
        } 
    }

    // Al salir de otros colliders, hacer al objeto colocable y cambiar su color al normal
    void OnTriggerExit2D(Collider2D other)
    {
        placeable = true; 
        if (tempRend != null) {
            tempRend.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, currentColor.a);
        }
    }
}

