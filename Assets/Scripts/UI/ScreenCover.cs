using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenCover : MonoBehaviour
{
    [SerializeField] private Image[] coverArray;
    private void Awake() {
        for (int i = 0; i < GameManager.numberOfPlayers.Value; i++) {
            coverArray[i].gameObject.SetActive(false);
        }
        GameManager.state.OnValueChanged += StateChange;
    }

    // Función de cambio de estado de juego
    private void StateChange(GameState prev, GameState curr){
        if (this != null) {
            if (curr == GameState.WinScreen) {
                for (int i = 0; i < coverArray.Length; i++) {
                    coverArray[i].gameObject.SetActive(false);
                }
            }
        }
    }

    
}
