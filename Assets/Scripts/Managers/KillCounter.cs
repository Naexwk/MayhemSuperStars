using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class KillCounter : MonoBehaviour
{

    public int[] killCounterArray = new int[4];

    private void Awake() {
        GameManager.state.OnValueChanged += StateChange;
    }

    // Función de cambio de estado de juego
    private void StateChange(GameState prev, GameState curr){
        if (curr == GameState.Round || curr == GameState.StartGame) {
            Array.Clear(killCounterArray, 0, killCounterArray.Length);
        }
    }

    public void AddKill(int playerNumber){
        killCounterArray[playerNumber] += 1;
    }
}
