using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BlowingUpScript : NetworkBehaviour
{
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag != "Bomb" && other is BoxCollider2D)
        {
            if (other.gameObject.GetComponent<NetworkObject>() != null && IsServer) {
                Debug.Log("a");
                other.gameObject.GetComponent<NetworkObject>().Despawn();
            }
            Destroy(other.gameObject);
            Invoke("DestroyGameObject", 0.5f);
        }
    }

    void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
