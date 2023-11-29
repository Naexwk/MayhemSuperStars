using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

// Controlador de la bala de queso, habilidad especial de Cheeseman
public class CheeseBullet : NetworkBehaviour
{
    public int bulletDamage = 3;
    private GameObject[] players;
    private Rigidbody2D rb;
    public int playerNumber;

    // Ignorar colisiones con jugadores
    void Start()
    {
        // Ignorar colisiones con jugadores
        rb = GetComponent<Rigidbody2D>();
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            Physics2D.IgnoreCollision(player.transform.GetComponent<CapsuleCollider2D>(), GetComponent<Collider2D>());
        }

        // Añadir rotacion
        rb.AddTorque((750f * Mathf.Deg2Rad) * rb.inertia, ForceMode2D.Impulse);

        StartCoroutine(SelfDestruct());
    }

    // Destruir el proyectil después de 5 segundos
    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(5);
        if (IsServer) {
            Destroy(this.gameObject);
        }
    }
    
}
