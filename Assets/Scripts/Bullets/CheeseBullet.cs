using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CheeseBullet : NetworkBehaviour
{
    public int bulletDamage = 3;
    private GameObject[] players;
    public Rigidbody2D rb;

    // Variable que determina si la bala es server-side o client-side
    public bool isFake;

    /*void Awake()
    {
        Physics.IgnoreLayerCollision(6, 6);
    }*/

    // Ignorar colisiones con jugadores
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            Physics2D.IgnoreCollision(player.transform.GetComponent<CapsuleCollider2D>(), GetComponent<Collider2D>());
        }

        rb.AddTorque((750f * Mathf.Deg2Rad) * rb.inertia, ForceMode2D.Impulse);

        StartCoroutine(selfDestruct());
    }

    // Destruir el proyectil después de 5 segundos
    private IEnumerator selfDestruct(){
        yield return new WaitForSeconds(5);
        if (IsServer) {
            Destroy(this.gameObject);
        }
    }

    // DEV: Debería atravesar enemigos
    
}
