using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

// Controlador de balas de jugador
public class PlayerBullet : NetworkBehaviour
{
    public int bulletDamage = 0;
    public float bulletSpeed = 30f;
    public Vector2 bulletDirection;
    public int playerNumber;

    // Al entrar en colisión con un muro o enemigo, destruir la bala
    // La funcion de recibir daño esta en el script del enemigo
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "MapBorders")
        {   
            Destroy(this.gameObject);
        }
    }

    
}
