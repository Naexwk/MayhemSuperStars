using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakePlayerBullet : MonoBehaviour
{
    // Script para balas client-side. No hacen nada.

    public float bulletSpeed = 30f;

    // Al tocar un muro o enemigo, destruir el objeto.
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Enemy")
        {

            Destroy(this.gameObject);
        }
    }

    
}
