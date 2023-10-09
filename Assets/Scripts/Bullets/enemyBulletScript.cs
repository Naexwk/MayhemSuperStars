using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyBulletScript : MonoBehaviour
{
    
    void OnTriggerEnter2D(Collider2D col) {
        // Destruir al tocar un muro
        if (col.gameObject.tag == "Wall")
        {
            Destroy(this.gameObject);
        }

        // Hacer daño al jugador al tocarlo
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerController>().GetHit();
            Destroy(this.gameObject);
        }
    }
}
