using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PropKillCounter : MonoBehaviour
{
    public int[] propKillCounterArray = new int[4];

    private void Awake() {
        GameManager.state.OnValueChanged += StateChange;
    }

    // Función de cambio de estado de juego
    private void StateChange(GameState prev, GameState curr){
        if (curr == GameState.Round || curr == GameState.StartGame) {
            Array.Clear(propKillCounterArray, 0, propKillCounterArray.Length);
        }
    }

    public void AddPropKill(int playerNumber){
        propKillCounterArray[playerNumber] += 1;
    }
}
