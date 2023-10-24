using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode; 

// Controlador de colisiones de objeto colocable del modo edición
public class BombEditModeScript : NetworkBehaviour
{
    // Renderer y color del objeto
    private Renderer tempRend;
    private Color currentColor;

    public bool placeable = false;  

    private List<Collider2D> colliders = new List<Collider2D>();
    private List<Color> colors = new List<Color>();

    // Inicializar renderer y color propio
    void Start()
    {
        tempRend = GetComponent<Renderer>();
        currentColor = tempRend.material.color; 
    }
    // Add to list of Colliders if not MapBorders, changes the colors for the other objects
    private void OnTriggerEnter2D (Collider2D other) {
        if (!colliders.Contains(other) && other.gameObject.tag != "MapBorders")
        {
            colliders.Add(other);
            Renderer rend = other.gameObject.GetComponent<Renderer>();
            colors.Add(rend.material.color);
            rend.material.color = new Color(255f, rend.material.color.g, rend.material.color.b, rend.material.color.a);
        }
    }

    // Mientras el prop a colocar se mantenga dentro del collider de un objeto colocado,
    // cambiar su color a rojo y hacerlo no colocable
    void OnTriggerStay2D(Collider2D other)
    {
        if (other is BoxCollider2D && other.gameObject.tag != "MapBorders" )
        {
            placeable = true; 
            float newGreen = 255f;
            tempRend.material.color = new Color(currentColor.r, newGreen, currentColor.b, currentColor.a);
        } 
    }


    // Al salir de otros colliders, hacer al objeto colocable y cambiar su color al normal
    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag != "MapBorders"){
            placeable = false; 
            tempRend.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, currentColor.a);
            other.gameObject.GetComponent<Renderer>().material.color = colors[colliders.IndexOf(other)];
            colors.RemoveAt(colliders.IndexOf(other));
            colliders.Remove(other);
        }
    }
}

