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
    public List<Collider2D> GetColliders () { return colliders; }

    public GameObject bombExplosion;

    // Inicializar renderer y color
    void Start()
    {
        tempRend = GetComponent<Renderer>();
        currentColor = tempRend.material.color; 
    }
    // Add to list of Colliders if not MapBorders
    private void OnTriggerEnter2D (Collider2D other) {
        if (!colliders.Contains(other) && other.gameObject.tag != "MapBorders")
        {
            colliders.Add(other);
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
            placeable = true; 
            tempRend.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, currentColor.a);
            colliders.Remove(other);
        }
    }
}

