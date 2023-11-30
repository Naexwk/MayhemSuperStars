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
        
        if (!colliders.Contains(other) && other.gameObject.tag != "MapBorders" && GetARenderer(other) != null && other is BoxCollider2D && other.GetComponent<TriggerEditModeController>() == null) 
        {
            Renderer rend = GetARenderer(other);
            colliders.Add(other);
            if(rend != null){
                colors.Add(rend.material.color);
                rend.material.color = new Color(255f, rend.material.color.g, rend.material.color.b, rend.material.color.a);
            }
        }
    }

    // Mientras el prop a colocar se mantenga dentro del collider de un objeto colocado,
    // cambiar su color a rojo y hacerlo no colocable
    void OnTriggerStay2D(Collider2D other)
    {
        if (other is BoxCollider2D && other.gameObject.tag != "MapBorders" && GetARenderer(other)!= null && other.GetComponent<TriggerEditModeController>() == null)
        {
            placeable = true; 
            float newGreen = 255f;
            tempRend.material.color = new Color(currentColor.r, newGreen, currentColor.b, currentColor.a);
        } 
    }


    // Al salir de otros colliders, hacer al objeto colocable y cambiar su color al normal
    void OnTriggerExit2D(Collider2D other)
    {
        if(other is BoxCollider2D && other.gameObject.tag != "MapBorders" && GetARenderer(other) != null && other.GetComponent<TriggerEditModeController>() == null){
            placeable = false; 
            tempRend.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, currentColor.a);
            GetARenderer(other).material.color = colors[colliders.IndexOf(other)];
            colors.RemoveAt(colliders.IndexOf(other));
            colliders.Remove(other);
        }
    }

    Renderer GetARenderer(Collider2D other){
        Renderer rend = null;
        if(other.gameObject.GetComponent<Renderer>() != null){
            rend = other.gameObject.GetComponent<Renderer>();
            return rend;
        } else if (other.gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>() != null){
            return other.gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>();
        }
        return null; 
    }
}

