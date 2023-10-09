using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerBullet : NetworkBehaviour
{
    public int bulletDamage = 0;
    public float bulletSpeed = 30f;
    public Vector2 bulletDirection;
    public GameObject fakeBulletPrefab;


    // Al entrar en colisión con un muro o enemigo, destruir la bala
    // El daño al enemigo está en el script de este.
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Enemy")
        {
            Destroy(this.gameObject);
        }
    }

    
}
