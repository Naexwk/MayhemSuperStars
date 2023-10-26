using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BlowingUpScript : NetworkBehaviour
{
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag != "Bomb" && GetComponent<Collider2D>().GetType().ToString() == "UnityEngine.BoxCollider2D")
        {
            Destroy(other.gameObject);
            Invoke("DestroyGameObject", 0.5f);
        }
    }

    void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
