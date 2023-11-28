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

    [SerializeField] private AnimationClip barFillAnim;
    [SerializeField] private Sprite headCheeseman, headSarge;
    [SerializeField] private TMP_Text roundText1, roundText2;
    
    // Referencia a espacios de leaderboard para los jugadores 3 y 4
    [SerializeField] private GameObject player3;
    [SerializeField] private GameObject player4;

    // Referencia a los campos de texto de nombres de jugadores y puntos
    [SerializeField] private TMP_Text[] playerNames;
    [SerializeField] private TMP_Text[] playerScores;
    [SerializeField] private GameObject[] playerProgressBar;
    [SerializeField] private RectTransform [] pointsTransform;
    [SerializeField] private Image [] characterHeads;

    [SerializeField] private string[] characterCodes;
    // Lista de jugadores
    private GameObject[] players;
    private GameObject[] deadplayers;

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

        UpdateRoundNumber(roundText1, roundText2, gameManager.currentRound, gameManager.maxRounds);

        // Recuperar la información de puntajes del GameManager
        for (int i = 0; i < gameManager.networkLeaderboard.Count; i++) {
            
            playerNames[i].text = gameManager.networkPlayerNames[gameManager.networkLeaderboard[i]].ToString();

            StartCoroutine(AnimateNumber(gameManager.networkPoints[gameManager.networkLeaderboard[i]], playerScores[i]));
            
            //Calcula la puntuacion mas alta
            float maxValue = gameManager.networkPoints[gameManager.networkLeaderboard[0]];
            if (gameManager.networkPoints[gameManager.networkLeaderboard[i]] > maxValue)
            {
                maxValue = gameManager.networkPoints[gameManager.networkLeaderboard[i]];
            }
            
            //Animacion de barra
            float fillAmountProgressBar = gameManager.networkPoints[gameManager.networkLeaderboard[i]]/maxValue;
            StartCoroutine(MovePointsYAxis(fillAmountProgressBar, pointsTransform[i]));
            BarFillAnimation(fillAmountProgressBar, barFillAnim, playerProgressBar[i]);

            players = GameObject.FindGameObjectsWithTag("Player");
            deadplayers = GameObject.FindGameObjectsWithTag("Dead Player");

            foreach (GameObject player in players)
            {
                if (Convert.ToInt32(player.GetComponent<PlayerController>().playerNumber) == gameManager.networkLeaderboard[i]) {
                    ProgressBarColorChanger(player.GetComponent<PlayerController>().characterCode.Value.ToString(), playerProgressBar[i].GetComponent<Image>(), characterHeads[i]);
                }
            }

            foreach (GameObject deadPlayer in deadplayers)
            {
                if (Convert.ToInt32(deadPlayer.GetComponent<PlayerController>().playerNumber) == gameManager.networkLeaderboard[i]) {
                    ProgressBarColorChanger(deadPlayer.GetComponent<PlayerController>().characterCode.Value.ToString(), playerProgressBar[i].GetComponent<Image>(), characterHeads[i]);
                }
            }
        }
    }

    public void ProgressBarColorChanger(string characterCode, Image progressBar, Image characterHead){
        if (characterCode == "cheeseman") {
            progressBar.color = new Color32(221,177,65,255);
            characterHead.sprite = headCheeseman;

        }
        if (characterCode == "sarge") {
            progressBar.color = new Color32(78,88,70,255);
            characterHead.sprite = headSarge;
        }
    }

    private void BarFillAnimation(float fillNumber,  AnimationClip barFillAnim , GameObject barObject)
    {
        if (float.IsNaN(fillNumber))
        {
            fillNumber = 0.1f;
        }
        Debug.Log("Entro aqui: "  +fillNumber);
        // Verifica si el componente Animation ya está adjunto al GameObject
        Animation anim = barObject.GetComponent<Animation>();

        // Si el componente Animation no está adjunto, agrégalo
        if (anim == null)
        {
            anim = barObject.AddComponent<Animation>();
        }

        // Configura la animación
        barFillAnim.legacy = true;
        AnimationCurve fillCurve = AnimationCurve.EaseInOut(0, 0, barFillAnim.length, fillNumber);
        barFillAnim.SetCurve("", typeof(Image), "m_FillAmount", fillCurve);

        // Añade o reemplaza el clip en el componente Animation
        anim.AddClip(barFillAnim, "ModifiedClip");

        // Reproduce la animación
        anim.Play("ModifiedClip");
    }

     
    IEnumerator AnimateNumber(int score, TMP_Text scoreText)
    {
        float elapsed = 0;
        float currentNumber = 0;
        float duration = 50f/60f; 

        while (elapsed < duration)
        {
            //Corregir Time.deltaTime por que va a tener problemas de multijugador
            elapsed += Time.deltaTime;
            currentNumber = Mathf.Lerp(0, score, elapsed / 0.83f);
            scoreText.text = Mathf.RoundToInt(currentNumber).ToString() + "K";
            yield return null;
        }
        
        scoreText.text = Mathf.RoundToInt(score).ToString() + "K";
    }

    IEnumerator MovePointsYAxis(float fillValue, RectTransform rectTransform){
        float elapsed = 0;
        float startY = -300;
        float endY = 153;
        float duration = 0.79f; 

        if (float.IsNaN(fillValue))
        {
            fillValue = 0.1f;
        }

        while (elapsed < duration)
        {
            float currentY = Mathf.Lerp(startY, endY, elapsed / duration * fillValue);
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, currentY);
            elapsed += Time.deltaTime;
            yield return null;
        }

        float finalY = startY + (fillValue * (endY - startY));
        Debug.Log(finalY);
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, finalY);
    }

    private void UpdateRoundNumber(TMP_Text text1, TMP_Text text2, int roundNumber, int maxRounds){
        if(roundNumber != maxRounds){
            text1.text = "ROUND 0" + roundNumber.ToString();
            text2.text = "ROUND 0" + roundNumber.ToString();
        } else{
            text1.text = "FINAL ROUND";
            text2.text = "FINAL ROUND";
        }
    }
}