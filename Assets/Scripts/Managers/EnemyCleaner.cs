using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controlador que limpia el escenario al terminar una ronda
public class EnemyCleaner : MonoBehaviour
{
    private GameObject[] enemies;

    // Escuchar cambios de estado de juego
    void Awake(){
        GameManager.state.OnValueChanged += StateChange;
    }

    // Eliminar a todos los enemigos al terminar una ronda
    private void StateChange(GameState prev, GameState curr){
        if (curr != GameState.Round && curr != GameState.StartGame) {
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies) {
                Destroy(enemy.gameObject);
            }
        }
    }
}
