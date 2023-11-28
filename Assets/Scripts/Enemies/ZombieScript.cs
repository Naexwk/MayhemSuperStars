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
    
    //Knockback variables
    private float knockbackForce = 15f;
    private float knockbackDuration = 0.2f;
    private Rigidbody2D rb;

    [SerializeField] private GameObject zombieDie;
    [SerializeField] private GameObject zombieHit;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
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
                ZombieGetHitServerRpc(col.gameObject.GetComponent<PlayerBullet>().bulletDamage, col.gameObject.GetComponent<PlayerBullet>().bulletDirection);
            } else if (col.gameObject.GetComponent<CheeseBullet>() != null) {
                ZombieGetHitServerRpc(col.gameObject.GetComponent<CheeseBullet>().bulletDamage, new Vector2(0,0));
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
    public void ZombieGetHitServerRpc(int damage, Vector2 direction) {
        StartCoroutine(ApplyKnockback(direction));
        health.Value -= damage;
        Instantiate(zombieHit, transform.position, transform.rotation);
        if (health.Value <= 0) {
            Instantiate(zombieDie, transform.position, transform.rotation);
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


    //Knockback
    private IEnumerator ApplyKnockback(Vector2 direction)
    {
        // Knockback direction
        Vector2 knockbackDirection = direction;

        // Apply a force to the enemy's Rigidbody2D
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        // Stop the knockback force after a duration
        rb.velocity = Vector2.zero;
    }
}
