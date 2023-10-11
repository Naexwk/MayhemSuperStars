using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.Netcode;

// Controlador de leaderboard
public class Leaderboard : MonoBehaviour
{
    // Referencia a GameManager
    private GameManager gameManager;
    
    // Referencia a espacios de leaderboard para los jugadores 3 y 4
    [SerializeField] private GameObject player3;
    [SerializeField] private GameObject player4;

    // Referencia a los campos de texto de nombres de jugadores y puntos
    [SerializeField] private TMP_Text[] playerNames;
    [SerializeField] private TMP_Text[] playerScores;

    // Actualizar leaderboard al activar (al finalizar cada ronda)
    void OnEnable()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        updateLeaderboard(true, true);
    }

    // Escuchar cambios de leaderboard
    void Start()
    {
        GameManager.handleLeaderboard.OnValueChanged += updateLeaderboard;
    }

    void OnDestroy()
    {
        GameManager.handleLeaderboard.OnValueChanged -= updateLeaderboard;
    }

    // Actualizar información del leaderboard
    public void updateLeaderboard(bool prev, bool curr){
        // Encender espacios según el número de jugadores
        if (GameManager.numberOfPlayers.Value >= 3) {
            player3.SetActive(true);
        }
        if (GameManager.numberOfPlayers.Value >= 4) {
            player4.SetActive(true);
        }

        // Recuperar la información de puntajes del GameManager
        for (int i = 0; i < gameManager.networkLeaderboard.Count; i++) {
            playerNames[i].text = gameManager.networkPlayerNames[gameManager.networkLeaderboard[i]].ToString();
            playerScores[i].text = "" + gameManager.networkPoints[gameManager.networkLeaderboard[i]];
        }
    }
}
