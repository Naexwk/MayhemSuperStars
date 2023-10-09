using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;

public class RoundManager : MonoBehaviour
{

    // Mostly deprecated
    
    private GameObject[] players;
    private GameObject[] cameraTargets;
    public GameObject PlayButton;
    private bool RoundTimer;
    public float RoundTime;
    public float currentRoundTime;
    public TMP_Text timeText;
/*
    public void activatePlayButton(bool _state)
    {
        PlayButton.SetActive(_state);
    }

    public void Update()
    {
        if (RoundTimer)
        {
            currentRoundTime -= Time.deltaTime;
            timeText.text = (Mathf.Round(currentRoundTime * 10.0f) / 10.0f).ToString();
            if (currentRoundTime <= 0)
        {
            GameManager.Instance.UpdateGameState(GameState.Leaderboard);
        }
        }
    }

    public void StartGame()
    {
        GameManager.Instance.UpdateGameState(GameState.StartGame);
        //players = GameObject.FindGameObjectsWithTag("Player");

        /*foreach (GameObject player in players)
        {
            player.GetComponent<PlayerController>().spawnPlayerClientRpc();
            player.GetComponent<PlayerController>().startCameraClientRpc();
        }*//*
         CombatRound();
    }

    public void CombatRound()
    {
        GameManager.Instance.UpdateGameState(GameState.Round);
        currentRoundTime = RoundTime;
        RoundTimer = true;

    }*/
}