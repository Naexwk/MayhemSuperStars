using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using UnityEngine.SceneManagement;

// Controlador de UI a nivel local
public class MenuManager : NetworkBehaviour
{
    // Elementos de UI a cargar
    private GameObject _lanScreen, _timer, _leaderboard, _purchaseScreen, _purchaseItemsUI, _purchaseTrapsUI, _healthHeartsUI, _specialAbilityUI;
    private TMP_Text _vidaText;
    private GameObject _winScreen;
    public GameObject heartPrefab;
    List<HealthHeart> hearts = new List<HealthHeart>();

    // Variables de control de fases
    private bool PurchasePhaseItems, PurchasePhaseTraps;  
    private bool purchased = false;

    // Lista de jugadores
    private GameObject[] players;
    public GameObject myPlayer;

    // Lista de objetivos de cámara
    private GameObject[] cameraTargets;
    private GameObject myCameraTarget;

    // Objeto que almacena los elementos de UI
    [SerializeField] private UIHelper uiHelper;
    private bool loaded = false;

    // Script de mi jugador
    private PlayerController myPlayerScript;

    // Variable de control de seguimiento de vida
    
    private Image cooldownRadial;
    //
    private bool startRecordingLife = false;

    // Destruir al entrar al character select, se regenerará al entrar a la escena de juego.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameRoom" && this != null) {
            Destroy(this.gameObject);
        }
    }

    // Suscribirse al cambio de estado del GameManager, al cambio de handleLeaderboard,
    // y a los cambios de escena
    void Awake(){
        GameManager.state.OnValueChanged += GameManagerOnGameStateChanged;
        GameManager.handleLeaderboard.OnValueChanged += UpdateLeaderboard;
        NetworkManager.SceneManager.OnSceneEvent += OnSceneEvent;
        SceneManager.sceneLoaded += OnSceneLoaded;
        loaded = false;
    }

    // Al entrar a escena de juego, cargar elementos de UI y gestionar al jugador
    void OnSceneEvent (SceneEvent sceneEvent) {
        if (sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted) {
            if (SceneManager.GetActiveScene().name == "SampleScene" && this != null){
                // Desplegar vida
                startRecordingLife = true;

                // Cargar elementos de UI
                uiHelper = GameObject.FindWithTag("UIHelper").GetComponent<UIHelper>();
                _timer = uiHelper.GameTimer;
                _leaderboard = uiHelper.Leaderboard;
                _purchaseScreen = uiHelper.PurchaseUI;
                _purchaseItemsUI = uiHelper.PurchaseItems;
                _purchaseTrapsUI = uiHelper.PurchaseTraps;
                _healthHeartsUI = uiHelper.HealthHearts;
                _vidaText = uiHelper.VidaText.GetComponent<TMP_Text>();
                _winScreen = uiHelper.winScreen;
                _specialAbilityUI = uiHelper.SpecialAbility;
                loaded = true;
                
                // Cargar acciones de botones
                LoadButtonActions();

                if (IsOwner) {
                    // Encontrar al jugador local
                    players = GameObject.FindGameObjectsWithTag("Player");
                    foreach (GameObject player in players) {
                        if (player.GetComponent<NetworkObject>().OwnerClientId == GetComponent<NetworkObject>().OwnerClientId){
                            myPlayer = player;
                            myPlayerScript = player.GetComponent<PlayerController>();
                        }
                    }
                    
                    // Encontrar cameraTarget local
                    cameraTargets = GameObject.FindGameObjectsWithTag("CameraTarget");
                    foreach (GameObject cameraTarget in cameraTargets) {
                        if (cameraTarget.GetComponent<NetworkObject>().OwnerClientId == GetComponent<NetworkObject>().OwnerClientId){
                            myCameraTarget = cameraTarget;
                        }
                    }
                }

                // Respawnear player
                if (myPlayer != null) {
                    myPlayer.GetComponent<PlayerController>().Respawn();
                }

                // Lockear camera al jugador
                if (myCameraTarget != null) {
                    myCameraTarget.GetComponent<CameraTarget>().lockOnPlayer = true;
                }
            }
        }

    }

    // Carga las acciones de los botones de seleccion de props y sponsors
    void LoadButtonActions(){
        Button button;
        button = _purchaseScreen.transform.GetChild(0).GetComponent<Button>();
        button.onClick.AddListener(OnSelectedItems);

        button = _purchaseScreen.transform.GetChild(1).GetComponent<Button>();
        button.onClick.AddListener(OnSelectedTraps);

        button = _purchaseItemsUI.transform.GetChild(0).GetComponent<Button>();
        button.onClick.AddListener(() => SelectObject(1));

        button = _purchaseItemsUI.transform.GetChild(1).GetComponent<Button>();
        button.onClick.AddListener(() => SelectObject(2));

        button = _purchaseItemsUI.transform.GetChild(2).GetComponent<Button>();
        button.onClick.AddListener(() => SelectObject(3));

        button = _purchaseTrapsUI.transform.GetChild(0).GetComponent<Button>();
        button.onClick.AddListener(SelectTrap);

        button = _purchaseTrapsUI.transform.GetChild(1).GetComponent<Button>();
        button.onClick.AddListener(SelectTrap);

        button = _purchaseTrapsUI.transform.GetChild(2).GetComponent<Button>();
        button.onClick.AddListener(SelectTrap);
    }


    // Seleccionar sponsors
    private void OnSelectedItems(){
        PurchasePhaseItems = true;
        GameManagerOnGameStateChanged(GameState.PurchasePhase, GameState.PurchasePhase);
    }

    // Añade el sponsor al jugador segun su ID
    private void SelectObject(int _objectID){
        if (IsOwner) {
            myPlayer.GetComponent<ItemManager>().addItem(_objectID);
            purchased = true;
            GameManagerOnGameStateChanged(GameState.PurchasePhase, GameState.PurchasePhase);
        }
    }

    // Seleccionar props
    private void OnSelectedTraps(){
        PurchasePhaseTraps = true;
        GameManagerOnGameStateChanged(GameState.PurchasePhase, GameState.PurchasePhase);
    }

    // Funcion para agregar prop a los botones de estos
    private void SelectTrap(){
        if (IsOwner) {
            purchased = true;
            GameManagerOnGameStateChanged(GameState.PurchasePhase, GameState.PurchasePhase);
        }
    }

    // Actualizar vida
    void Update (){ 
        if (IsOwner && startRecordingLife) {
            //_vidaText.GetComponent<TMP_Text>().text = ("Health: " + myPlayerScript.currentHealth);
            DrawHearts();
            specialAbility();
        }
        
    }

    // Función de cambio de estado de juego
    private void GameManagerOnGameStateChanged(GameState prev, GameState curr){
        if (!loaded || !IsOwner) {
            return;
        }

        if (this == null) {
            return;
        }

        // Activar o desactivar elementos de UI según el estado del juego
        _timer.SetActive(curr == GameState.StartGame || curr == GameState.Round || curr == GameState.PurchasePhase || curr == GameState.PurchasePhase);
        _leaderboard.SetActive(curr == GameState.Leaderboard);
        _winScreen.SetActive(curr == GameState.WinScreen);
        _healthHeartsUI.gameObject.SetActive(curr == GameState.Round || curr == GameState.StartGame);

        // Administrar spawns del jugador y movimientos de cámara
        if(curr != GameState.Round && curr != GameState.StartGame) {
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

        // Gestionar UI al seleccionar props o sponsors
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

    // Función para llamar la actualización del leaderboard
    private void UpdateLeaderboard(bool prev, bool curr){
        if (_leaderboard != null) {
            if(_leaderboard.activeSelf){
                _leaderboard.GetComponent<Leaderboard>().UpdateLeaderboard(true, true);
            }
        }
        
    }

    public void DrawHearts()
    {
        ClearHearts();

        float maxHealthReminder = myPlayerScript.maxHealth % 2;
        int heartsToMake = (int)((myPlayerScript.maxHealth /2) + maxHealthReminder);

        for(int i = 0; i< heartsToMake; i++)
        {
            CreateEmptyHeart();
        }

        for(int i = 0; i< hearts.Count; i++)
        {
            int heartStatusRemainder = (int)Mathf.Clamp(myPlayerScript.currentHealth - (i*2), 0, 2);
            hearts[i].SetHeartImage((HeartStatus)heartStatusRemainder);
        }
    }

    public void CreateEmptyHeart()
    {
        GameObject newHeart = Instantiate(heartPrefab);
        newHeart.transform.SetParent(_healthHeartsUI.transform);

        HealthHeart heartCompenent = newHeart.GetComponent<HealthHeart>();
        heartCompenent.SetHeartImage(HeartStatus.Empty);
        hearts.Add(heartCompenent);
    }

    public void ClearHearts()
    {
        foreach(Transform t in _healthHeartsUI.transform)
        {
            Destroy(t.gameObject);
        }
        hearts = new List<HealthHeart>();
    }

    public void specialAbility()
    {
        cooldownRadial = _specialAbilityUI.transform.GetChild(0).gameObject.GetComponent<Image>();
        float radialFillAmount = ((Time.time - myPlayerScript.timeSinceLastAbility)/myPlayerScript.abilityCooldown);
        
        cooldownRadial.fillAmount = radialFillAmount;

        GameObject  specialAbiliyImage = _specialAbilityUI.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
        UI_SpecialAb specialAbComponent= specialAbiliyImage.GetComponent<UI_SpecialAb>();
        string charCode = myPlayerScript.characterCode.Value.ToString();

        Debug.Log(radialFillAmount);

        specialAbComponent.SetSpecialAbImage(charCode, radialFillAmount);
    }

}
