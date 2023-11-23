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
    private GameObject _lanScreen, _timer, _leaderboard, _purchaseScreen, _purchaseItemsUI, _purchaseTrapsUI, _healthHeartsUI, _specialAbilityUI, _sponsorsUI, _countdownUI;
    private TMP_Text _vidaText;
    private GameObject _winScreen;
    private GameObject _canvas;

    //Variables UI Health Hearts

    private GameObject _optionsSelector;
    public GameObject heartPrefab;
    List<HealthHeart> hearts = new List<HealthHeart>();

    //Variables UI Sponsors
    public GameObject sponsorPrefab;
    List<UI_Sponsors> sponsors = new List<UI_Sponsors>();

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
    private ItemManager itemManagerScript;

    // Variable de control de seguimiento de vida
    
    private Image cooldownRadial;
    //
    private bool startRecordingLife = false;

    [SerializeField] private GameObject prefabCanvas;
    private GameObject myCanvas;
    
    private ItemController itemController;

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
        myCanvas = Instantiate(prefabCanvas);
        loaded = false;
        
    }

    // Al entrar a escena de juego, cargar elementos de UI y gestionar al jugador
    void OnSceneEvent (SceneEvent sceneEvent) {
        if (sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted) {
            if (SceneManager.GetActiveScene().name == "SampleScene" && this != null){
                // Desplegar vida
                startRecordingLife = true;

                // Cargar elementos de UI
                //uiHelper = GameObject.FindWithTag("UIHelper").GetComponent<UIHelper>();
                uiHelper = myCanvas.GetComponent<UIHelper>();
                _canvas = myCanvas;
                _timer = uiHelper.GameTimer;
                _leaderboard = uiHelper.Leaderboard;
                _purchaseScreen = uiHelper.PurchaseUI;
                _purchaseItemsUI = uiHelper.PurchaseItems;
                _purchaseTrapsUI = uiHelper.PurchaseTraps;
                _healthHeartsUI = uiHelper.HealthHearts;
                _vidaText = uiHelper.VidaText.GetComponent<TMP_Text>();
                _winScreen = uiHelper.winScreen;
                _optionsSelector = uiHelper.optionsSelector;
                _specialAbilityUI = uiHelper.SpecialAbility;
                _sponsorsUI = uiHelper.Sponsors;
                _countdownUI = uiHelper.Countdown;
                loaded = true;
                
                
                
                // Cargar acciones de botones
                LoadButtonActions();

                if (IsOwner) {
                    myPlayerScript = myPlayer.GetComponent<PlayerController>();
                    itemManagerScript = myPlayer.GetComponent<ItemManager>();
                    
                    // Encontrar cameraTarget local
                    if (myPlayerScript.cameraTarget != null) {
                        myCameraTarget = myPlayerScript.cameraTarget;
                    } else {
                        cameraTargets = GameObject.FindGameObjectsWithTag("CameraTarget");
                        foreach (GameObject cameraTarget in cameraTargets) {
                            if (cameraTarget.GetComponent<NetworkObject>().OwnerClientId == GetComponent<NetworkObject>().OwnerClientId){
                                myCameraTarget = cameraTarget;
                            }
                        }
                    }
                }

                if (uiHelper.lem != null) {
                    Debug.Log("LEM 1: " + uiHelper.lem.cameraReference);
                    uiHelper.lem.cameraReference = myPlayerScript.playerCamera;
                    Debug.Log("Player XD: " + myPlayerScript.playerCamera);
                    Debug.Log("LEM 2: " +uiHelper.lem.cameraReference);
                    if (uiHelper.itemController != null) { 
                        uiHelper.lem.itemController = uiHelper.itemController;
                    }
                }

                if (uiHelper.itemController != null) {
                    itemController = uiHelper.itemController;
                    if (uiHelper.lem != null) {
                        itemController.editor = uiHelper.lem;
                    }
                    if (_optionsSelector != null) {
                        itemController.optionsSelector = _optionsSelector.GetComponent<OptionsSelector>();
                    }
                    uiHelper.itemController.cameraReference = myPlayerScript.playerCamera;
                }

                // Respawnear player
                if (myPlayer != null) {
                    myPlayer.GetComponent<PlayerController>().Respawn();
                }

                // Lockear camera al jugador
                if (myCameraTarget != null) {
                    myCameraTarget.GetComponent<CameraTarget>().lockOnPlayer = true;
                }

                if (myPlayer.GetComponent<PlayerController>().playerCamera != null) {
                    _canvas.GetComponent<Canvas>().worldCamera = myPlayer.GetComponent<PlayerController>().playerCamera;
                }

                _optionsSelector.GetComponent<OptionsSelector>().myPlayer = myPlayerScript;
            }
        }

    }

    // Carga las acciones de los botones de seleccion de props y sponsors
    void LoadButtonActions(){
        Button button;
        button = _purchaseScreen.transform.GetChild(0).GetComponent<Button>();
        button.onClick.AddListener(OnSelectedTraps);

        button = _purchaseScreen.transform.GetChild(1).GetComponent<Button>();
        button.onClick.AddListener(OnSelectedItems);

        button = _purchaseItemsUI.transform.GetChild(0).GetComponent<Button>();
        button.onClick.AddListener(() => SelectObject(0));

        button = _purchaseItemsUI.transform.GetChild(1).GetComponent<Button>();
        button.onClick.AddListener(() => SelectObject(1));

        button = _purchaseItemsUI.transform.GetChild(2).GetComponent<Button>();
        button.onClick.AddListener(() => SelectObject(2));

        button = _purchaseTrapsUI.transform.GetChild(0).GetComponent<Button>();
        button.onClick.AddListener(SelectTrap);
        button.onClick.AddListener(() => ItemControllerHelper(0));

        button = _purchaseTrapsUI.transform.GetChild(1).GetComponent<Button>();
        button.onClick.AddListener(SelectTrap);
        button.onClick.AddListener(() => ItemControllerHelper(1));

        button = _purchaseTrapsUI.transform.GetChild(2).GetComponent<Button>();
        button.onClick.AddListener(SelectTrap);
        button.onClick.AddListener(() => ItemControllerHelper(2));


    }

    private void ItemControllerHelper (int id) {
        itemController.StartNewPlacement(id, myPlayer);
    }

    // Seleccionar sponsors
    private void OnSelectedItems(){
        PurchasePhaseItems = true;
        GameManagerOnGameStateChanged(GameState.PurchasePhase, GameState.PurchasePhase);
    }

    // Añade el sponsor al jugador segun su ID
    private void SelectObject(int _objectID){
        if (IsOwner) {
            GameObject gm;
            gm = GameObject.FindGameObjectWithTag("GameManager");
            gm.GetComponent<GameManager>().DoneWithPurchaseServerRpc();
            myPlayer.GetComponent<ItemManager>().addItem(_optionsSelector.GetComponent<OptionsSelector>().sponsorOptions[_objectID]);
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
            DrawSponsor();
            specialAbility();
        }
        
    }

    // Función de cambio de estado de juego
    private void GameManagerOnGameStateChanged(GameState prev, GameState curr){
        GameObject gm;
        gm = GameObject.FindGameObjectWithTag("GameManager");
        GameManager gameManager = gm.GetComponent<GameManager>();

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
        _specialAbilityUI.gameObject.SetActive(curr == GameState.Round || curr == GameState.StartGame);
        _sponsorsUI.gameObject.SetActive(curr == GameState.Round || curr == GameState.StartGame);
        _countdownUI.gameObject.SetActive(curr == GameState.Countdown || curr == GameState.TimesUp);
        // Administrar spawns del jugador y movimientos de cámara
        if(curr != GameState.Round && curr != GameState.StartGame) {
            if (curr != GameState.Countdown && curr != GameState.TimesUp) {
                if (myPlayer != null) {
                    myPlayer.GetComponent<PlayerController>().Despawn();
                }
                if (myCameraTarget != null) {
                    myCameraTarget.GetComponent<CameraTarget>().lockOnPlayer = false;
                }
            } else {
                if (myCameraTarget != null) {
                    myCameraTarget.GetComponent<CameraTarget>().lockOnPlayer = true;
                }
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

    //Health Hearts Functions
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

        HealthHeart heartComponent = newHeart.GetComponent<HealthHeart>();
        heartComponent.SetHeartImage(HeartStatus.Empty);
        hearts.Add(heartComponent);
    }

    public void ClearHearts()
    {
        foreach(Transform t in _healthHeartsUI.transform)
        {
            Destroy(t.gameObject);
        }
        hearts = new List<HealthHeart>();
    }



    //Sponsors Functions
    public void DrawSponsor()
    {
        ClearSponsors();
        int sponsorsToMake; 
        sponsorsToMake = itemManagerScript.itemIDs.Length;

        for(int i = 0; i< sponsorsToMake; i++)
        {
            CreateEmptySponsor();
        }

        for(int i=0; i< sponsors.Count; i++){
            sponsors[i].SetSponsorImg((SponsorStatus)itemManagerScript.itemIDs[i]);
        }
    }

    public void CreateEmptySponsor()
    {
        GameObject newSponsor = Instantiate(sponsorPrefab);
        newSponsor.transform.SetParent(_sponsorsUI.transform);

        UI_Sponsors sponsorComponent = newSponsor.GetComponent<UI_Sponsors>();
        sponsorComponent.SetSponsorImg(SponsorStatus.SausageHeart);
        sponsors.Add(sponsorComponent);
    }

    public void ClearSponsors()
    {
        foreach(Transform t in _sponsorsUI.transform)
        {
            Destroy(t.gameObject);
        }
        sponsors = new List<UI_Sponsors>();
    }

    public void specialAbility()
    {
        cooldownRadial = _specialAbilityUI.transform.GetChild(0).gameObject.GetComponent<Image>();
        float radialFillAmount = ((Time.time - myPlayerScript.timeSinceLastAbility)/myPlayerScript.abilityCooldown);
        
        cooldownRadial.fillAmount = radialFillAmount;

        GameObject  specialAbiliyImage = _specialAbilityUI.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
        UI_SpecialAb specialAbComponent= specialAbiliyImage.GetComponent<UI_SpecialAb>();
        string charCode = myPlayerScript.characterCode.Value.ToString();

        specialAbComponent.SetSpecialAbImage(charCode, radialFillAmount);
    }

}
