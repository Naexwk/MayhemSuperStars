using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCleaner : MonoBehaviour
{
    private GameObject[] enemies;
    void Awake(){
        GameManager.State.OnValueChanged += StateChange;
    }

    private void StateChange(GameState prev, GameState curr){
        if (curr != GameState.Round && curr != GameState.StartGame) {
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies) {
                Destroy(enemy.gameObject);
            }
        }
    }
}
