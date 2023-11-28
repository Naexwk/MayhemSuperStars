using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    int counter;

    public void Autodestroy () {
        counter++;
        if (counter >= 2 && this != null) {
            Destroy(this.gameObject, 0.2f);
        }
        
    }
}
