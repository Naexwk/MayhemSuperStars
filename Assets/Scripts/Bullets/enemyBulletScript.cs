using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controlador de las balas enemigas
public class enemyBulletScript : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision) {
        // Destruir al tocar un muro
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "MapBorders")
        {
            Destroy(this.gameObject);
        }

        // Hacer daño al jugador al tocarlo
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().GetHit(GetComponent<damageSource>().owner);
            Destroy(this.gameObject);
        }

        if (collision.gameObject.tag == "PlayerBullet" && collision.gameObject.GetComponent<CheeseBullet>() != null)
        {
            Destroy(this.gameObject);
        }
    }
}
