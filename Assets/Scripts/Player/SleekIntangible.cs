using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleekIntangible : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other) {
        if (!other.gameObject.CompareTag("MapBorders")) {
            Physics2D.IgnoreCollision(GetComponent<CapsuleCollider2D>(), other.gameObject.GetComponent<Collider2D>());
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("MapBorders")) {
            Physics2D.IgnoreCollision(GetComponent<CapsuleCollider2D>(), other);
        }
    }
}
