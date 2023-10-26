using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controlador de las balas falsas, unicamente como efecto visual.
public class FakePlayerBullet : MonoBehaviour
{

    public float bulletSpeed = 30f;

    // Al tocar un muro o enemigo, destruir el objeto.
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "MapBorders")
        {
            Destroy(this.gameObject);
        }
    }

    
}
