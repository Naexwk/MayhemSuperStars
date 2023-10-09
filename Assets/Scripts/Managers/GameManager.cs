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
    public static GameManager Instance;
    //public GameState State;
    public static NetworkVariable<GameState> State = new NetworkVariable<GameState>(default, NetworkVariableReadPermission.Everyone);

    public static event Action<GameState> OnGameStateChanged;
    public NetworkVariable<bool> GameStarted = new NetworkVariable<bool>();
    public bool startHandled = false;
    #endregion

    #region Round Manager Variables   
    private GameObject[] players;
    private GameObject[] cameraTargets;

    public NetworkVariable<bool> RoundSection = new NetworkVariable<bool>();
    public NetworkVariable<bool> LeaderboardSection = new NetworkVariable<bool>();
    public NetworkVariable<bool> PurchasePhase = new NetworkVariable<bool>();

    //private bool RoundSection, LeaderboardSection, PurchasePhase;
    public float RoundTime, leaderboardTime, purchaseTime;

    // Variables de tiempo
    public NetworkVariable<float> currentRoundTime = new NetworkVariable<float>();
    public NetworkVariable<float> currentLeaderboardTime = new NetworkVariable<float>();
    public NetworkVariable<float> currentPurchaseTime = new NetworkVariable<float>();

    private TMP_Text timeText;


    #endregion

    // Variables de rondas
    static public int numberOfPlayers;
    public static NetworkVariable<bool> changedPlayers = new NetworkVariable<bool>();

    private int[] points = {4,8,8,16,32};
    private int currentRound;
    private int maxRounds = 5;

    private int[] playerPoints;
    private int[] playerLeaderboard;
    private int[] helper;

    public NetworkList<int> networkPoints;
    public NetworkList<int> networkLeaderboard;
    public NetworkList<FixedString64Bytes> networkPlayerNames;

    public static NetworkVariable<bool> handleLeaderboard = new NetworkVariable<bool>(default, NetworkVariableReadPermission.Everyone);


    void Awake() {
        Instance = this;
        networkPoints = new NetworkList<int>();
        networkLeaderboard = new NetworkList<int>();
        networkPlayerNames = new NetworkList<FixedString64Bytes>();
        handleLeaderboard.Value = false;
        //SceneManager.sceneLoaded += OnSceneLoaded;
        NetworkManager.SceneManager.OnSceneEvent += OnSceneEvent;
        //State = this.State;
        
    }
    
    void Start() {
        //Debug.Log(SceneManager.GetActiveScene().name);
        UpdateGameState(GameState.LanConnection);
    }
    
    /*private void OnEnable() {
        Debug.Log(SceneManager.GetActiveScene().name);
        Debug.Log("Called");
        if (SceneManager.GetActiveScene().name == "SampleScene"){
            StartGame();
            HandleStartGame();
        }
    }*/

    void OnSceneEvent (SceneEvent sceneEvent) {
        if (sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted) {
            //Debug.Log ("Called OnSync");
            if (SceneManager.GetActiveScene().name == "SampleScene"){
                GameStarted.Value = false;
                UpdateGameState(GameState.LanConnection);
                StartGame();
                HandleStartGame();
            }

            //if (SceneManager.GetActiveScene().name == "GameRoom"){
                //StartGame();
                //HandleStartGame();
                
            //}
        }

    }

    /*void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SampleScene") {
            
        }
    }*/
    

    public void AddPlayer(string _name){

        numberOfPlayers++;
        changedPlayers.Value = !changedPlayers.Value;
        networkPlayerNames.Add(_name);
        Debug.Log("Added " + _name);
    }

    private void updateScores(){
        players = GameObject.FindGameObjectsWithTag("Player");
        int pointsToDistribute;
        if (players.Length != 0) {
            pointsToDistribute = points[currentRound-1] / players.Length;
        } else {
            pointsToDistribute = 0;
        }
        //Debug.Log("points for today: " + points[currentRound-1]);
        //Debug.Log("Players.Length: " + players.Length);
        //Debug.Log("Points to Distribute: " + pointsToDistribute);
        foreach (GameObject player in players)
        {
            playerPoints[Convert.ToInt32(player.GetComponent<PlayerController>().playerNumber)] += pointsToDistribute;
        }

        networkPoints.Clear();
        for (int i = 0; i < playerPoints.Length; i++) {
            networkPoints.Add(playerPoints[i]);
        }

        //Debug.Log("helper length: " + helper.Length);
        //prevPlayerPoints = playerPoints;
        Array.Copy(playerPoints, helper, numberOfPlayers);
        //Debug.Log("helper length 2: " + helper.Length);
        //helper = playerPoints;
        playerLeaderboard = SortAndIndex(helper);
        
        Array.Reverse(playerLeaderboard);
        //playerPoints = prevPlayerPoints;

        /*for (int i = 0; i < playerLeaderboard.Length; i++) {
            Debug.Log("order: " + playerLeaderboard[i]);
        }*/
        
        networkLeaderboard.Clear();
        
        for (int i = 0; i < playerLeaderboard.Length; i++) {
            networkLeaderboard.Add(playerLeaderboard[i]);
        }

        handleLeaderboard.Value = !handleLeaderboard.Value;
        //Debug.Log(handleLeaderboard.Value);
    }

    static int[] SortAndIndex<T>(T[] rg)
    {
        int i, c = rg.Length;
        var keys = new int[c];
        if (c > 1)
        {
            for (i = 0; i < c; i++)
                keys[i] = i;

            System.Array.Sort(rg, keys /*, ... */);
        }
        return keys;
    }

    public override void OnNetworkSpawn()
    {
        
        base.OnNetworkSpawn();
        // Modifica las NetworkVariables y realiza otras configuraciones aquí
        GameStarted.Value = false; // Por ejemplo, establece GameStarted en false cuando se inicie el NetworkObject
        DontDestroyOnLoad(this.gameObject);
        UpdateGameState(GameState.LanConnection);
        
    }


    public void Update()
    {

        if(GameStarted.Value){
            if(!startHandled){
                GameManager.Instance.UpdateGameState(GameState.StartGame);
                startHandled = true;
            }
        }

        if (RoundSection.Value)
        {
            //Debug.Log("RoundSection.Value!");
            TMP_Text timeText = FindTimerText();
            if (IsOwner) {
                currentRoundTime.Value -= Time.deltaTime;
            }

            if (timeText != null) {
                timeText.text = (Mathf.Round(currentRoundTime.Value * 10.0f) / 10.0f).ToString();
            }
            
            if(currentRoundTime.Value <= 10.1)
            {
                if (timeText != null) {
                    timeText.text = Mathf.Round(currentRoundTime.Value ).ToString();
                    timeText.color = Color.red;
                    timeText.fontSize = 70;
                }
                
            }
            if (currentRoundTime.Value <= 0.5 )
            {
                GameManager.Instance.UpdateGameState(GameState.Leaderboard);
                if (IsOwner) {
                    RoundSection.Value = false;
                }

            }
        }
        if (LeaderboardSection.Value)
        {
            //Debug.Log("Leaderboard!");
            if (IsOwner) {
                currentLeaderboardTime.Value -= Time.deltaTime;
            }
            
            if (currentLeaderboardTime.Value <= 1)
            {
                currentRound++;
                if (currentRound > maxRounds) {
                    GameManager.Instance.UpdateGameState(GameState.WinScreen);
                } else {
                    GameManager.Instance.UpdateGameState(GameState.PurchasePhase);
                }
                if (IsOwner) {
                    LeaderboardSection.Value= false;
                }
                
            }
        }

        if (PurchasePhase.Value)
        {
            TMP_Text timeText = FindTimerText();
            //Hacer esta parte bien
            timeText.color = Color.white;
            timeText.fontSize = 50;
            //Hacer esta parte bien
            if (IsOwner) {
                currentPurchaseTime.Value -= Time.deltaTime;
            }
            if (timeText != null) {
                timeText.text = Mathf.Round(currentPurchaseTime.Value).ToString();
            }
            
            //Debug.Log(currentPurchaseTime.Value);
            if (currentPurchaseTime.Value <= 0)
            {
                if (IsOwner) {
                    PurchasePhase.Value = false;
                }
                ResetValues();
                // Llamar al RPC
                CombatRound();
            }
        }
    }

    public void UpdateGameState(GameState newState){
        if (IsOwner) {
            State.Value = newState;
        }
        Debug.Log("State: " + newState);
        switch(newState){
            case GameState.LanConnection:
                HandleLanConnection();
                break;
            case GameState.StartGame:
                HandleStartGame();
                break;
            case GameState.Round:
                HandleRound();
                break;
            case GameState.Leaderboard:
                HandleLeaderboard();
                break;
            case GameState.PurchasePhase:
                HandlePurchasePhase();
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

    private void HandleLanConnection(){

    }
    
    #region Round Manager Functions

    public void StartGame()
    {
        if (IsServer) {
            //Debug.Log("numberofplayers: " + numberOfPlayers);
            playerPoints = new int[numberOfPlayers];
            helper = new int[numberOfPlayers];
            playerLeaderboard = new int[numberOfPlayers];
            currentRound = 1;
            //Debug.Log(playerPoints.Length);
            GameStarted.Value = true;
        }
        
    }

    public void CombatRound()
    {
        /*currentRound++;
        if (currentRound > maxRounds) {
            GameManager.Instance.UpdateGameState(GameState.WinScreen);
        } else {
            GameManager.Instance.UpdateGameState(GameState.Round);
        }*/

        GameManager.Instance.UpdateGameState(GameState.Round);
    }

    private void HandleStartGame(){

        if (IsOwner) {
            players = GameObject.FindGameObjectsWithTag("Player");
            //Debug.Log(players.Length);
            foreach (GameObject player in players)
            {
                //player.GetComponent<PlayerController>().spawnPlayerClientRpc();
                player.GetComponent<PlayerController>().startCameraClientRpc();
            }
            currentRoundTime.Value = RoundTime;
            currentLeaderboardTime.Value = leaderboardTime;
            currentPurchaseTime.Value = purchaseTime;
        }
        CombatRound();
    }
    private void HandleRound(){
        if (IsOwner) {
            RoundSection.Value = true;

        }
        
    }
    void HandleLeaderboard(){
        if (IsOwner) {
            LeaderboardSection.Value = true;
            updateScores();
        }
    }
    #endregion

    void HandlePurchasePhase(){
        
        if (IsOwner) {
            
            PurchasePhase.Value = true;
        }
    }

    void ResetValues(){
        if (IsOwner) {
            currentRoundTime.Value = RoundTime;
            currentLeaderboardTime.Value = leaderboardTime;
            currentPurchaseTime.Value = purchaseTime;
        }
        
        
    }

    public TMP_Text FindTimerText()
    {
        GameObject textMeshProObject = GameObject.FindWithTag("Timer");
        if (textMeshProObject != null)
        {
            TMP_Text textMeshProComponent = textMeshProObject.GetComponent<TMP_Text>();
            if (textMeshProComponent != null)
            {
                return textMeshProComponent;
            }
            else
            {
                Debug.LogError("TextMeshPro component not found on the object with the 'Timer' tag.");
                return null;
            }
        }
        else
        {
            Debug.LogError("GameObject with the 'Timer' tag not found.");
            return null;
        }
    }


}


public enum GameState{
    LanConnection,
    StartGame,
    Round,
    Leaderboard,
    PurchasePhase,
    EditMode,
    WinScreen
};