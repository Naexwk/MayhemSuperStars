using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

using UnityEngine.SceneManagement;

public class MenuManager : NetworkBehaviour
{
    [SerializeField] private GameObject _lanScreen, _timer, _leaderboard, _purchaseScreen, _purchaseItemsUI, _purchaseTrapsUI;
    [SerializeField] private TMP_Text _vidaText;
    [SerializeField] private GameObject _winScreen;
    private bool PurchasePhaseItems, PurchasePhaseTraps;  
    private bool purchased = false;

    private GameObject[] players;
    public GameObject myPlayer;
    private GameObject[] cameraTargets;
    public GameObject myCameraTarget;
    public GameObject UIHelper;
    public bool loaded = false;

    private PlayerController myPlayerScript;
    bool startRecordingLife = false;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameRoom" && this != null) {
            Destroy(this.gameObject);
        }
    }


    // Suscribirse al cambio de estado del GameManager
    void Awake(){
        
        GameManager.State.OnValueChanged += GameManagerOnGameStateChanged;
        GameManager.handleLeaderboard.OnValueChanged += updateLeaderboard;
        NetworkManager.SceneManager.OnSceneEvent += OnSceneEvent;
        SceneManager.sceneLoaded += OnSceneLoaded;
        loaded = false;
    }

    // Encuentra al jugador al que le corresponda este MenuManager
    void Start()
    {

        
    }

    void OnSceneEvent (SceneEvent sceneEvent) {
        if (sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted) {
            //Debug.Log ("Called OnSync");
            Debug.Log("Loaded Scene in MM");
                if (SceneManager.GetActiveScene().name == "SampleScene" && this != null){
                    Debug.Log("Started Loading in MM");
                    startRecordingLife = true;
                    UIHelper = GameObject.FindWithTag("UIHelper");

                    //_lanScreen = UIHelper.GetComponent<UIHelper>().LanScreen;
                    _timer = UIHelper.GetComponent<UIHelper>().GameTimer;
                    _leaderboard = UIHelper.GetComponent<UIHelper>().Leaderboard;
                    _purchaseScreen = UIHelper.GetComponent<UIHelper>().PurchaseUI;
                    _purchaseItemsUI = UIHelper.GetComponent<UIHelper>().PurchaseItems;
                    _purchaseTrapsUI = UIHelper.GetComponent<UIHelper>().PurchaseTraps;
                    _vidaText = UIHelper.GetComponent<UIHelper>().VidaText.GetComponent<TMP_Text>();
                    _winScreen = UIHelper.GetComponent<UIHelper>().winScreen;

                    loaded = true;
                    

                    loadButtonActions();

                    if (IsOwner) {
                        
                        players = GameObject.FindGameObjectsWithTag("Player");
                        foreach (GameObject player in players) {
                            if (player.GetComponent<NetworkObject>().OwnerClientId == GetComponent<NetworkObject>().OwnerClientId){
                                myPlayer = player;
                                myPlayerScript = player.GetComponent<PlayerController>();
                            }
                        }

                        cameraTargets = GameObject.FindGameObjectsWithTag("CameraTarget");
                        foreach (GameObject cameraTarget in cameraTargets) {
                            if (cameraTarget.GetComponent<NetworkObject>().OwnerClientId == GetComponent<NetworkObject>().OwnerClientId){
                                myCameraTarget = cameraTarget;
                            }
                        }
                        //StartCoroutine(searchForCameraTarget());
                    }

                    if (myPlayer != null) {
                        myPlayer.GetComponent<PlayerController>().Respawn();
                    }
                    if (myCameraTarget != null) {
                        myCameraTarget.GetComponent<CameraTarget>().lockOnPlayer = true;
                    }
            }
        }

    }

    void loadButtonActions(){
        Button button;
        button = _purchaseScreen.transform.GetChild(0).GetComponent<Button>();
        button.onClick.AddListener(OnSelectedItems);

        button = _purchaseScreen.transform.GetChild(1).GetComponent<Button>();
        button.onClick.AddListener(OnSelectedTraps);

        button = _purchaseItemsUI.transform.GetChild(0).GetComponent<Button>();
        button.onClick.AddListener(() => selectObject(1));

        button = _purchaseItemsUI.transform.GetChild(1).GetComponent<Button>();
        button.onClick.AddListener(() => selectObject(2));

        button = _purchaseItemsUI.transform.GetChild(2).GetComponent<Button>();
        button.onClick.AddListener(() => selectObject(3));

        button = _purchaseTrapsUI.transform.GetChild(0).GetComponent<Button>();
        button.onClick.AddListener(selectTrap);

        button = _purchaseTrapsUI.transform.GetChild(1).GetComponent<Button>();
        button.onClick.AddListener(selectTrap);

        button = _purchaseTrapsUI.transform.GetChild(2).GetComponent<Button>();
        button.onClick.AddListener(selectTrap);
    }
/*
    void OnDestroy() {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }*/

    // Funciones de objetos
    public void OnSelectedItems(){
        PurchasePhaseItems = true;
        GameManagerOnGameStateChanged(GameState.PurchasePhase, GameState.PurchasePhase);
    }

    // Añade el objeto al jugador segun su ID
    public void selectObject(int _objectID){
        if (IsOwner) {
            myPlayer.GetComponent<ItemManager>().addItem(_objectID);
            purchased = true;
            GameManagerOnGameStateChanged(GameState.PurchasePhase, GameState.PurchasePhase);
        }
    }

    public void OnSelectedTraps(){
        PurchasePhaseTraps = true;
        GameManagerOnGameStateChanged(GameState.PurchasePhase, GameState.PurchasePhase);
        //Codigo de trampas
    }

    public void selectTrap(){
        if (IsOwner) {
            purchased = true;
            GameManagerOnGameStateChanged(GameState.PurchasePhase, GameState.PurchasePhase);
        }
    }

    void Update (){ 
        if (IsOwner && startRecordingLife) {
            _vidaText.GetComponent<TMP_Text>().text = ("Vida: " + myPlayerScript.currentHealth);
        }
        
    }


    private void GameManagerOnGameStateChanged(GameState prev, GameState curr){
        if (!loaded || !IsOwner) {
            Debug.Log("Haven't Loaded!");
            return;
        }

        if (this == null) {
            return;
        }

        //_lanScreen.SetActive(curr == GameState.LanConnection);
        _timer.SetActive(curr == GameState.StartGame || curr == GameState.Round || curr == GameState.PurchasePhase || curr == GameState.PurchasePhase);
        _leaderboard.SetActive(curr == GameState.Leaderboard);
        _winScreen.SetActive(curr == GameState.WinScreen);
        /*if (curr == GameState.Leaderboard) {
            _leaderboard.GetComponent<Leaderboard>().distributePoints();
        }*/
        _vidaText.gameObject.SetActive(curr == GameState.Round || curr == GameState.StartGame);
        if(curr != GameState.Round && curr != GameState.StartGame) {;
            if (myPlayer != null) {
                myPlayer.GetComponent<PlayerController>().Despawn();
            }
            if (myCameraTarget != null) {
                myCameraTarget.GetComponent<CameraTarget>().lockOnPlayer = false;
            }
        } else {
            if (myPlayer != null) {
                myPlayer.GetComponent<PlayerController>().Respawn();
            }
            if (myCameraTarget != null) {
                myCameraTarget.GetComponent<CameraTarget>().lockOnPlayer = true;
            }
        }

        if (PurchasePhaseTraps || PurchasePhaseItems) {
            _purchaseScreen.SetActive(false);
            if(curr == GameState.PurchasePhase){
                if (purchased) {
                    _purchaseItemsUI.SetActive(false);
                    _purchaseTrapsUI.SetActive(false);
                } else {
                    if(PurchasePhaseItems == true){
                        _purchaseItemsUI.SetActive(true);
                    }
                    else{
                        _purchaseTrapsUI.SetActive(true);
                    }
                }
             }
             else{
                _purchaseItemsUI.SetActive(false);
                _purchaseTrapsUI.SetActive(false);
                PurchasePhaseTraps = false;
                PurchasePhaseItems = false;
                purchased = false;
            }
        } else {
            _purchaseScreen.SetActive(curr == GameState.PurchasePhase);
        }
    }


    private void updateLeaderboard(bool prev, bool curr){
        if (_leaderboard != null) {
            if(_leaderboard.activeSelf){
                _leaderboard.GetComponent<Leaderboard>().updateLeaderboard(true, true);
            }
        }
        
    }


}
