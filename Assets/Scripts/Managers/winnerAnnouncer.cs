using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Controlador de la pantalla de victoria
public class winnerAnnouncer : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private TMP_Text winnerText;

    // Al inicializar el panel, obtener el nombre del ganador y desplegar
    void OnEnable()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        winnerText.text = (gameManager.networkPlayerNames[gameManager.networkLeaderboard[0]].ToString()) + " IS THE";
    }

}
