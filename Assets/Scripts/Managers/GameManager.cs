using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Collections;

public class GameManager : NetworkBehaviour
{
    #region Game Manager Variables
    public static GameManager instance;
    // Variable de estado de juego
    public static NetworkVariable<GameState> state = new NetworkVariable<GameState>(default, NetworkVariableReadPermission.Everyone);
    public static event Action<GameState> OnGameStateChanged;
    // Variables de control de inicio de juego
    public NetworkVariable<int> readyPlay = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> gameStarted = new NetworkVariable<bool>();
    private bool startHandled = false;
    #endregion

    #region Round Manager Variables
    // Lista de jugadores
    private GameObject[] players;
    private GameObject[] cameraTargets;

    // Variables de control de estados de juego
    public NetworkVariable<bool> roundSection = new NetworkVariable<bool>();
    public NetworkVariable<bool> leaderboardSection = new NetworkVariable<bool>();
    public NetworkVariable<bool> purchasePhase = new NetworkVariable<bool>();

    // Variables de tiempo de estados de juego
    [SerializeField] private float roundTime, leaderboardTime, purchaseTime, countdownTime;

    // Variables de tiempo
    public NetworkVariable<float> currentRoundTime = new NetworkVariable<float>();
    public NetworkVariable<float> currentLeaderboardTime = new NetworkVariable<float>();
    public NetworkVariable<float> currentPurchaseTime = new NetworkVariable<float>();
    
    // CountdownUI Component
    private GameObject countdownUI;

    #endregion

    // Variables de jugadores
    static public NetworkVariable<int> numberOfPlayers = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone);
    static public NetworkVariable<bool> changedPlayers = new NetworkVariable<bool>();

    // Puntos por ronda
    private int[] points = {4,8,8,16,32};
    public NetworkVariable<int> done = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<int> deadPlayers = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone);

    // Variables de control de rondas
    public int currentRound;
    private int maxRounds = 5;

    // Arrays de ayuda para almacenar y ordenar los puntos del jugador
    private int[] playerPoints;
    private int[] playerLeaderboard;
    private int[] helper;

    // Listas de puntos de jugador, desordenados y ordenados
    public NetworkList<int> networkPoints;
    public NetworkList<int> networkLeaderboard;

    // Lista de nombres de jugador
    public NetworkList<FixedString64Bytes> networkPlayerNames;

    // Variable de control para llamar a actualizar el leaderboard
    public static NetworkVariable<bool> handleLeaderboard = new NetworkVariable<bool>(default, NetworkVariableReadPermission.Everyone);

    // Variable para acceder al animator del timer
    
    private GameObject[] textMeshProObject;

    //private TMP_Text[] timeTextArray;
    private List<TMP_Text> timeTextArray = new List<TMP_Text>();

    // Inicializar valores
    void Awake() {
        instance = this;
        networkPoints = new NetworkList<int>();
        networkLeaderboard = new NetworkList<int>();
        networkPlayerNames = new NetworkList<FixedString64Bytes>();
        handleLeaderboard.Value = false;
        NetworkManager.SceneManager.OnSceneEvent += OnSceneEvent;
    }
    
    // Entrar a estado de juego neutral
    void Start() {
        UpdateGameState(GameState.LanConnection);
    }

    // Al cargar la escena de juego, reiniciar los valores de juego
    void OnSceneEvent (SceneEvent sceneEvent) {
        if (sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted) {
            if (SceneManager.GetActiveScene().name == "SampleScene" && IsOwner){
                FindTimerText();
                gameStarted.Value = false;
                UpdateGameState(GameState.LanConnection);
                StartGame();
                HandleStartGame();
            }
        }
    }
    
    // Añadir jugador a la red
    public void AddPlayer(string name){
        numberOfPlayers.Value++;
        Debug.Log(numberOfPlayers.Value);
        changedPlayers.Value = !changedPlayers.Value;
        networkPlayerNames.Add(name);
        Debug.Log("Added name: " + name);
    }

    public void RemovePlayer(string name){
        numberOfPlayers.Value--;
        Debug.Log(numberOfPlayers.Value);
        changedPlayers.Value = !changedPlayers.Value;
        networkPlayerNames.Remove(name);
        Debug.Log("Removed name: " + name);
    }

    public void ReadyPlay(){
        readyPlay.Value++;
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void ReadyPlayServerRpc(){
        ReadyPlay();
    }

    public void NotReadyPlay(){
        readyPlay.Value--;
    }

    [ServerRpc(RequireOwnership = false)]
    public void NotReadyPlayServerRpc(){
        NotReadyPlay();
    }

    public void DoneWithPurchase(){
        done.Value++;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DoneWithPurchaseServerRpc(){
        DoneWithPurchase();
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayerDiedServerRpc(){
        PlayerDied();
    }

    public void PlayerDied(){
        deadPlayers.Value++;
    }

    // Actualizar los puntos de los jugadores
    private void UpdateScores(){
        // Distribuir los puntos de ronda entre los jugadores vivos
        players = GameObject.FindGameObjectsWithTag("Player");
        int pointsToDistribute;
        if (players.Length != 0) {
            pointsToDistribute = points[currentRound-1] / players.Length;
        } else {
            pointsToDistribute = 0;
        }
        foreach (GameObject player in players)
        {
            playerPoints[Convert.ToInt32(player.GetComponent<PlayerController>().playerNumber)] += pointsToDistribute;
        }

        // Enviar los puntos de cada jugador a la red a través de networkPoints
        networkPoints.Clear();
        for (int i = 0; i < playerPoints.Length; i++) {
            networkPoints.Add(playerPoints[i]);
        }

        // Ordenar los puntos de los jugadores
        Array.Copy(playerPoints, helper, numberOfPlayers.Value);
        playerLeaderboard = SortAndIndex(helper);
        Array.Reverse(playerLeaderboard);

        // Enviar el orden de los jugadores para el leaderboard a la red a través de networkLeaderboard
        networkLeaderboard.Clear();
        for (int i = 0; i < playerLeaderboard.Length; i++) {
            networkLeaderboard.Add(playerLeaderboard[i]);
        }

        // Llamar a actualizar el leaderboard
        handleLeaderboard.Value = !handleLeaderboard.Value;
    }

    // Función para ordenar un array y recibir uno nuevo con la posición original
    // de los datos.
    static int[] SortAndIndex<T>(T[] rg)
    {
        int i, c = rg.Length;
        var keys = new int[c];
        if (c > 1)
        {
            for (i = 0; i < c; i++)
                keys[i] = i;

            System.Array.Sort(rg, keys);
        }
        return keys;
    }

    // Al aparecer en la red
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        gameStarted.Value = false;
        DontDestroyOnLoad(this.gameObject);
        UpdateGameState(GameState.LanConnection);
    }


    public void Update()
    {
        // Controlar si el juego inició
        if(gameStarted.Value){
            if(!startHandled){
                GameManager.instance.UpdateGameState(GameState.StartGame);
                startHandled = true;
            }
        }

        // Lógica de ronda de juego
        if (roundSection.Value)
        {
            // Actualizar el tiempo de ronda
            if (IsOwner) {
                currentRoundTime.Value -= Time.deltaTime;
                if (deadPlayers.Value >= numberOfPlayers.Value && currentRoundTime.Value > 3.6f) {
                    currentRoundTime.Value = 3.6f;
                    deadPlayers.Value = 0;
                }
            }
            foreach (TMP_Text timeText in timeTextArray) {
                Material timeTextMaterial = timeText.materialForRendering;
                //timeText.enableVertexGradient = false;
                timeText.color = Color.white;
                timeText.fontSize = 60f;
                Color colorOutline = new Color(49f / 255f, 49f / 255f, 49f / 255f);
                timeTextMaterial.SetColor(ShaderUtilities.ID_OutlineColor, colorOutline);
                timeTextMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.5f);

                // Actualizar el texto según el tiempo
                if (timeText != null) {
                    timeText.text = (Mathf.Round(currentRoundTime.Value * 10.0f) / 10.0f).ToString();
                }
                
                // Cambiar el formato del texto al quedar menos 10 segundos
                if(currentRoundTime.Value <= 10.6) 
                {
                    foreach (GameObject item in textMeshProObject){
                        item.GetComponent<Animator>().Play("timerAnim");
                    }
                    if (timeText != null) {
                        timeText.text = Mathf.Round(currentRoundTime.Value).ToString();
                        timeText.color = new Color(197f / 255f, 22f / 255f, 55f / 255f);
                        Color colorCountdown = new Color(27f / 255f, 0f / 255f, 8f / 255f);
                        timeTextMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.3f);
                        timeTextMaterial.SetColor(ShaderUtilities.ID_OutlineColor, colorCountdown);
                    }
                }

                // TimesUp
                if (currentRoundTime.Value < 0.6 )
                {
                    GameManager.instance.UpdateGameState(GameState.TimesUp);
                    if (IsOwner) {
                        roundSection.Value = false;
                    }
                }
            }
        } 

        // Lógica de leaderboard
        if (leaderboardSection.Value)
        {
            // Actualizar el tiempo de ronda
            if (IsOwner) {
                currentLeaderboardTime.Value -= Time.deltaTime;
            }
            
            // Al acabarse el tiempo, cambiar a WinScreen o a Fase de Compra según 
            // el número de rondas
            if (currentLeaderboardTime.Value <= 1)
            {
                currentRound++;
                if (currentRound > maxRounds) {
                    GameManager.instance.UpdateGameState(GameState.WinScreen);
                } else {
                    GameManager.instance.UpdateGameState(GameState.PurchasePhase);
                }
                if (IsOwner) {
                    leaderboardSection.Value = false;
                }
            }
        }

        // Lógica de fase de compra
        if (purchasePhase.Value)
        {
            // Actualizar el tiempo de fase de compra
            if (IsOwner) { 
                // CHANGE NEEDED HERE
                if(done.Value == numberOfPlayers.Value){
                    currentPurchaseTime.Value = 3.0f;
                    done.Value = 0;
                } else {
                    currentPurchaseTime.Value -= Time.deltaTime;
                }
            }

            foreach (TMP_Text timeText in timeTextArray)
            {
                // Modificar el texto
                timeText.fontSize = 60f;
                Color colorOutline = new Color(49f / 255f, 49f / 255f, 49f / 255f);
                Material timeTextMaterial = timeText.materialForRendering;
                timeTextMaterial.SetColor(ShaderUtilities.ID_OutlineColor, colorOutline);
                timeTextMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.5f);
                timeText.color = Color.white;

                

                // Actualizar el texto del timer
                if (timeText != null) {
                    timeText.text = Mathf.Round(currentPurchaseTime.Value).ToString();
                }
                
                // Cambiar a ronda de combate al acabarse el tiempo
                if (currentPurchaseTime.Value <= 0)
                {
                    if (IsOwner) {
                        purchasePhase.Value = false;
                    }
                    ResetValues();
                    // Llamar al RPC
                    CombatRound();
                }
            }
            
        }
    }

    // Función de cambio de estado de juego
    public void UpdateGameState(GameState newState){
        if (IsOwner) {
            state.Value = newState;
        }
        switch(newState){
            case GameState.LanConnection:
                break;
            case GameState.StartGame:
                HandleStartGame();
                break;
            case GameState.Countdown:
                HandleCountdown();
                break;
            case GameState.Round:
                HandleRound();
                break;
            case GameState.TimesUp:
                HandleTimesUp();
                break;
            case GameState.Leaderboard:
                HandleLeaderboard();
                break;
            case GameState.PurchasePhase:
                HandlepurchasePhase();
                break;
            case GameState.EditMode:
                break;
            case GameState.WinScreen:
                break;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
    }
    
    #region Round Manager Functions

    // Función de inicio de juego
    // Reinicia los valores de la partida
    public void StartGame()
    {
        if (IsServer) {
            playerPoints = new int[numberOfPlayers.Value];
            helper = new int[numberOfPlayers.Value];
            playerLeaderboard = new int[numberOfPlayers.Value];
            currentRound = 1;
            gameStarted.Value = true;
        }
        
    }

    // Inicio de la ronda
    public void CombatRound()
    {
        GameManager.instance.UpdateGameState(GameState.Countdown);
        //Aqui Codigo de Inicio de Ronda
    }

    // Función complementaria de inicio de juego
    // Inicia las cámaras de los jugadores y resetea los tiempos de las rondas
    private void HandleStartGame(){

        if (IsOwner) {
            players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                player.GetComponent<PlayerController>().StartCameraClientRpc();
            }
            ResetValues();
        }
        CombatRound();
    }

    private void HandleCountdown(){
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerController>().enableControl = false;
            player.GetComponent<PlayerController>().isInvulnerable = true;
        }
            StartCoroutine(Countdown(4.0f));
    }

    // Iniciar sección de ronda
    private void HandleRound(){
        if (IsOwner) {
            deadPlayers.Value = 0;
            roundSection.Value = true;
        }
    }
    
    private void HandleTimesUp(){
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerController>().enableControl = false;
            player.GetComponent<PlayerController>().isInvulnerable = true;
        }
        StartCoroutine(TimesUp(2.0f));
    }

    // Iniciar sección de leaderboard y actualizar puntos
    void HandleLeaderboard(){
        if (IsOwner) {
            leaderboardSection.Value = true;
            UpdateScores();
        }
    }
    
    // Iniciar sección de fase de compra
    void HandlepurchasePhase(){
        if (IsOwner) {
            done.Value = 0;
            purchasePhase.Value = true;
        }
    }
    #endregion

    // Reinicia los contadores de tiempo
    void ResetValues(){
        if (IsOwner) {
            currentRoundTime.Value = roundTime;
            currentLeaderboardTime.Value = leaderboardTime;
            currentPurchaseTime.Value = purchaseTime;
        }
    }

    // Función para encontrar el timer
    public void FindTimerText()
    {
        textMeshProObject = GameObject.FindGameObjectsWithTag("Timer");
        
        if (textMeshProObject.Length != 0)
        {

            foreach(GameObject tmpObject in textMeshProObject) {
                timeTextArray.Add(tmpObject.GetComponent<TMP_Text>());
            }

            if (timeTextArray.Count == 0)
            {
                Debug.LogError("TextMeshPro component not found on the object with the 'Timer' tag.");
            }
        }
        else
        {
            Debug.LogError("GameObject with the 'Timer' tag not found.");
        }
    }

    IEnumerator Countdown(float time){
        GameObject[] countdownUIArray = GameObject.FindGameObjectsWithTag("Countdown");
        foreach (GameObject countdownUI in countdownUIArray) {
            countdownUI.GetComponent<Animator>().Play("321goAnim");
        }
        yield return new WaitForSeconds(time);
        foreach (GameObject countdownUI in countdownUIArray) {
            foreach (Transform child in countdownUI.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        GameManager.instance.UpdateGameState(GameState.Round);
    }

    IEnumerator TimesUp(float time){
        GameObject[] countdownUIArray = GameObject.FindGameObjectsWithTag("Countdown");
        foreach (GameObject countdownUI in countdownUIArray) {
            countdownUI.GetComponent<Animator>().Play("countdownAnim");
        }
        yield return new WaitForSeconds(time);
        foreach (GameObject countdownUI in countdownUIArray) {
            foreach (Transform child in countdownUI.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        GameManager.instance.UpdateGameState(GameState.Leaderboard);
    }
}


// Estados de juego
public enum GameState{
    LanConnection,
    StartGame,
    Countdown,
    Round,
    TimesUp,
    Leaderboard,
    PurchasePhase,
    EditMode,
    WinScreen
};