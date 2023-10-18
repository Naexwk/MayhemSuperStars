using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BlowingUpScript : NetworkBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("ON COLLISION ENTER");
        Destroy(collision.gameObject);
    }
}
