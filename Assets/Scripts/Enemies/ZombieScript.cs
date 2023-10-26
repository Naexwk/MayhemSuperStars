using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

// Controlador del enemigo "Zombie"
public class ZombieScript : NetworkBehaviour
{
    // Vida del zombie
    private NetworkVariable<int> health = new NetworkVariable<int>();

    private Animator animator;
    private SpriteRenderer m_SpriteRenderer;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        animator = GetComponent<Animator>();
        m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        // Aleatorizar el color del zombie
        m_SpriteRenderer.color = new Color(1f, 1f, Random.Range(0f, 1f));
        // Inicializar vida
        if (NetworkManager.Singleton.IsServer) {
            health.Value = 5;
        }
    }

    // Al entrar en contacto con una bala, recibir daño
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "PlayerBullet")
        {
            if (col.gameObject.GetComponent<PlayerBullet>() != null) {
                ZombieGetHitServerRpc(col.gameObject.GetComponent<PlayerBullet>().bulletDamage);
            } else if (col.gameObject.GetComponent<CheeseBullet>() != null) {
                ZombieGetHitServerRpc(col.gameObject.GetComponent<CheeseBullet>().bulletDamage);
            }
            
        }
    }

    // Al entrar en contacto con un jugador, dañarlo
    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerController>().GetHit();
        }
    }

    // Actualizar la vida en la red
    [ServerRpc(RequireOwnership = false)]
    public void ZombieGetHitServerRpc(int damage) {
        health.Value -= damage;
        if (health.Value <= 0) {
            Destroy(this.gameObject);
        }
    }

    private void Update() {
        if (this.gameObject.GetComponent<RunnerAI>().target == transform) {
            animator.SetBool("isMoving", false);
        } else {
            animator.SetBool("isMoving", true);
        }
    }
}
