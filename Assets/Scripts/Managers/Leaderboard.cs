using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using Unity.Netcode;
public class Leaderboard : MonoBehaviour
{
    private GameManager gameManager;
    private GameObject[] players;
    private GameObject[] deadPlayers;
    
   
    [SerializeField] private GameObject player3;
    [SerializeField] private GameObject player4;
    [SerializeField] private TMP_Text[] playerNames;
    [SerializeField] private TMP_Text[] playerScores;

    void OnEnable()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        updateLeaderboard(true, true);
    }

    void Start()
    {
        GameManager.handleLeaderboard.OnValueChanged += updateLeaderboard;
    }

    void OnDestroy()
    {
        GameManager.handleLeaderboard.OnValueChanged -= updateLeaderboard;
    }

    public void updateLeaderboard(bool prev, bool curr){
        //Debug.Log("hello: " + GameManager.numberOfPlayers);
        if (GameManager.numberOfPlayers >= 3) {
            player3.SetActive(true);
        }
        if (GameManager.numberOfPlayers >= 4) {
            player4.SetActive(true);
        }
        //Debug.Log("count");
        //Debug.Log(gameManager.networkLeaderboard.Count);
        for (int i = 0; i < gameManager.networkLeaderboard.Count; i++) {
            //Debug.Log("networkLeaderboard");
            //Debug.Log(gameManager.networkLeaderboard[i]);
            //playerNames[i].text = "Player " + ((gameManager.networkLeaderboard[i])+1);
            playerNames[i].text = gameManager.networkPlayerNames[gameManager.networkLeaderboard[i]].ToString();
            playerScores[i].text = "" + gameManager.networkPoints[gameManager.networkLeaderboard[i]];
        }
    }


}
