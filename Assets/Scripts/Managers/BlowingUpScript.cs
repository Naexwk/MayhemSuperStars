using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BlowingUpScript : NetworkBehaviour
{
    void OnTriggerStay2D(Collider2D other){
        if(other.gameObject.tag != "Bomb"){
            Debug.Log("On Trigger Stay " + other.gameObject);
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
    }
}
