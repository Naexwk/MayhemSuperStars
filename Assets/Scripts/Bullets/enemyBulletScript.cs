using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controlador de las balas enemigas
public class enemyBulletScript : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision) {
        // Destruir al tocar un muro
        if (collision.gameObject.tag == "Wall")
        {
            Destroy(this.gameObject);
        }

        // Hacer daño al jugador al tocarlo
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().GetHit();
            Destroy(this.gameObject);
        }
    }
}
