using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.Netcode;
using UnityEngine.UI;

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
    [SerializeField] private Image[] playerProgress;

    [SerializeField] private string[] characterCodes;
    // Lista de jugadores
    private GameObject[] players;


    // void Update() {
    //     players = GameObject.FindGameObjectsWithTag("Player");
    //     foreach (GameObject player in players) {
    //         player.GetComponent<PlayerController>().characterCode.Value.ToString();
    //         Debug.Log(player.GetComponent<PlayerController>().characterCode.Value.ToString());
    //     }
        
    // }

    void Awake() {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    // Actualizar información del leaderboard
    public void UpdateLeaderboard(bool prev, bool curr){
        // Buscar gameManager
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        // Activar espacios según el número de jugadores
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
            float maxValue = gameManager.networkPoints[gameManager.networkLeaderboard[0]];
            if (gameManager.networkPoints[gameManager.networkLeaderboard[i]] > maxValue)
            {
                maxValue = gameManager.networkPoints[gameManager.networkLeaderboard[i]];
            }
            float fillAmountProgressBar = gameManager.networkPoints[gameManager.networkLeaderboard[i]]/maxValue;
            playerProgress[i].fillAmount = fillAmountProgressBar;

            ProgressBarColorChanger(players[i].GetComponent<PlayerController>().characterCode.Value.ToString(), playerProgress[i]);
        }
    }

    public void ProgressBarColorChanger(string characterCode, Image progressBar){
        if (characterCode == "cheeseman") {
            progressBar.color = new Color32(221,177,65,255);
        }
        if (characterCode == "sarge") {
            progressBar.color = new Color32(78,88,70,255);
        }
    }
}
