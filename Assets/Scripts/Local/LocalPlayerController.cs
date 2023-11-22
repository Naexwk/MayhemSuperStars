using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.Collections;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

public class LocalPlayerController : PlayerController
{
    // Estadísticas del personaje
    private float char_playerSpeed = 8f;
    private float char_bulletSpeed = 30f;
    private int char_maxHealth = 6;
    private int char_fireRate = 3; // en disparos por segundo
    private int char_bulletDamage = 3;

    // Estadísticas de jugador actuales
    public float bulletSpeed;

    //Animacion
    [SerializeField] private RuntimeAnimatorController[] characterAnimators;

    // Variables de control
    private float timeSinceLastFire;
    public int abilityDamage;
    [SerializeField] private float invulnerabilityWindow;
    public bool sargeActive = false;

    // Variables de personaje
    public GameObject bubble;
    // public string characterCode = "cheeseman";
    specialAbility specAb;

    // Objetos para movimiento
    private Rigidbody2D rig;

    // Objetos de cámara
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject cameraTargetPrefab;

    // Objetos de Network
    private GameObject bullethandler;
    [SerializeField] private GameObject prefabMenuManager;

    // Variables visuales
    private GameObject outline;
    
    // Spawn points
    // DEV: Hacer GameObjects para modificarlos en escena
    private Vector3[] spawnPositions = { new Vector3(56.5f,28f,0f), new Vector3(57.5f,14.5f,0f), new Vector3(29f,13.5f,0f), new Vector3(23.5f,30f,0f) };
    //private Vector3[] spawnPositions = { new Vector3(65.83f,36.37f,0f), new Vector3(67f,22.5f,0f), new Vector3(38.24f,21.71f,0f), new Vector3(32.7f,38.65f,0f) };

    private GameManager gameManager;

    // Input variables
    private bool input_Shoot, input_Special;
    private Vector2 input_Movement;

    private int deviceID;
    [SerializeField] private GameObject prefabCharSelCanvas;

    // Función para colorear objetos según el número del jugador
    public void ColorCodeToPlayer (GameObject go, ulong playerNumber) {
        if (playerNumber == 0) {
            go.GetComponent<Renderer>().material.color = Color.red;
        }
        if (playerNumber == 1) {
            go.GetComponent<Renderer>().material.color = Color.blue;
        }
        if (playerNumber == 2) {
            go.GetComponent<Renderer>().material.color = Color.yellow;
        }
        if (playerNumber == 3) {
            go.GetComponent<Renderer>().material.color = Color.green;
        }
    }

    // Inicializar controladores de jugador
    async void Start()
    {
        deviceID = GetComponent<PlayerInput>().devices[0].deviceId;
        GetComponent<NetworkObject>().Spawn();
        playerNumber = Convert.ToUInt64(GameManager.numberOfPlayers.Value);
        string name = "P" + (playerNumber+1);
        //bubble = transform.GetChild(0).gameObject;
        DontDestroyOnLoad(this.gameObject);
        bubble.GetComponent<SpriteRenderer>().enabled = false;
        rig = gameObject.GetComponent<Rigidbody2D>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        gameManager.AddPlayer(name);
        GameObject relayManager = GameObject.FindWithTag("RelayManager");

        // DEV: Reañadir outline
        outline = gameObject.transform.GetChild(0).gameObject;
        if (IsOwner) {
            // Inicializar como personaje default (cheeseman)
            await ChangeCharacter("cheeseman");
        }
        
    }

    // Escuchar cambios de escena
    private void Awake() {
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameManager.state.OnValueChanged += StateChange;
        GameObject canvas;
        canvas = Instantiate(prefabCharSelCanvas, new Vector3(0f,0f,0f), transform.rotation);
        canvas.GetComponent<Canvas>().worldCamera = playerCamera;
    }

    // Ejecutar funciones de escena de juego
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SampleScene") {
            bullethandler = GameObject.FindWithTag("BulletHandler");
            if (IsOwner) {
                animator.SetBool("dead", false);
                gameObject.tag = "Player";
                ChangeDeadStateServerRpc(false, playerNumber);
                SpawnCameraTargetServerRpc(playerNumber);
                SpawnMenuManagerServerRpc(playerNumber);
            }
            
        }
    }

    // Función de cambio de estado de juego
    private void StateChange(GameState prev, GameState curr){
        // Si empieza una ronda de juego, permitir al jugador usar su habilidad especial
        if (this != null) {
            if (curr == GameState.Countdown) {
                Respawn();
            }

            if (curr == GameState.Round || curr == GameState.StartGame) {
                timeSinceLastAbility = Time.time - abilityCooldown;
                this.isInvulnerable = false;
                StopCoroutine(recordInvulnerabiltyFrames());
                StartCoroutine(recordInvulnerabiltyFrames());
                InputSystem.ResumeHaptics();
            } else {
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(0f,0f,0f);
                enableControl = false;
                this.isInvulnerable = true;
                InputSystem.ResetHaptics();
            }
        }
    }

    // Input functions

    public void OnMove(InputAction.CallbackContext context){
        input_Movement = context.ReadValue<Vector2>();
    }

    public void OnShoot(InputAction.CallbackContext context){
        input_Shoot = context.action.triggered;
    }

    public void OnSpecial(InputAction.CallbackContext context){
        input_Special = context.action.triggered;
    }

    public void OnShootDirection(InputAction.CallbackContext context){
        input_ShootDirection = context.ReadValue<Vector2>();
    }

    void Update()
    {
        // Si no es dueño de este script o no está habilitado
        // el  control, ignorar
        

        if (!IsOwner || !enableControl) {
            return;
        }

        // Disparar con el clic izquierdo
        if (input_Shoot)
        {
            Vector2 direction;
            if (GetComponent<PlayerInput>().devices[0].ToString() == "Keyboard:/Keyboard" || GetComponent<PlayerInput>().devices[0].ToString() == "Mouse:/Mouse") {
                Vector3 worldMousePos = mainCamera.ScreenToWorldPoint(input_ShootDirection);
                direction = worldMousePos - transform.position;
            } else {
                direction = input_ShootDirection;
            }
            
            Shoot(direction);
        }

        // Usar habilidad especial con el clic derecho
        if (input_Special)
        {
            CastSpecialAbility();
        }
        
    }

    private void FixedUpdate(){
        // Si no es dueño de este script o no está habilitado
        // el  control, ignorar
        if (!IsOwner || !enableControl) {
            return;
        }

        Move();
    }

    // Maneja el movimiento del jugador
    private void Move(){
        // Obtener input de jugador
        // Escucha WASD y flechas
        Vector2 direction;
        direction = input_Movement;
        direction.Normalize();
        direction = direction * playerSpeed;
        rig.velocity = direction;

        // Añadir velocidad al animador según la velocidad
        animator.SetFloat("Speed", Mathf.Abs(rig.velocity.magnitude));
    } 

    // Dispara una bala si ya se cumplió el tiempo de espera del firerate.
    private void Shoot(Vector2 direction){
        if ((Time.time - timeSinceLastFire) > (1f/fireRate)) {
            // Obtener dirección de disparo
            direction.Normalize();

            // Disparar en red
            if (bullethandler == null){
                return;
            }
            bullethandler.GetComponent<BulletHandler>().SpawnBulletServerRpc(bulletSpeed, direction, playerNumber, transform.position.x, transform.position.y);

            // Disparar a nivel local
            GameObject clone;
            clone = Instantiate(bullethandler.GetComponent<BulletHandler>().prefabBullet, transform.position, transform.rotation);
            clone.GetComponent<PlayerBullet>().bulletDamage = bulletDamage;
            clone.GetComponent<PlayerBullet>().bulletSpeed = bulletSpeed;
            clone.GetComponent<PlayerBullet>().bulletDirection = direction;
            clone.GetComponent<Rigidbody2D>().velocity = (direction) * (bulletSpeed);

            // Colorear según el jugador
            ColorCodeToPlayer(clone, playerNumber);

            // Actualizar
            timeSinceLastFire = Time.time;
        }
    }

    // Usa la habilidad especial, que depende del personaje
    private void CastSpecialAbility(){
        if ((Time.time - timeSinceLastAbility) > (abilityCooldown)) {
            specAb();
        }
    }

    // Funciones de habilidades especiales

    // Cheeseman: Aparece una bola de queso que daña a los enemigos
    private void CheesemanSA () {
        Vector2 direction;
        if (GetComponent<PlayerInput>().devices[0].ToString() == "Keyboard:/Keyboard" || GetComponent<PlayerInput>().devices[0].ToString() == "Mouse:/Mouse") {
            Vector3 worldMousePos = mainCamera.ScreenToWorldPoint(input_ShootDirection);
            direction = worldMousePos - transform.position;
        } else {
            direction = input_ShootDirection;
            if (direction == Vector2.zero) {
                return;
            }
        }
        // Encontrar dirección de disparo
        
        direction.Normalize();

        // Instanciar bala en red
        bullethandler.GetComponent<BulletHandler>().SpawnCheeseBulletServerRpc(direction, playerNumber, abilityDamage, transform.position.x, transform.position.y);
        
        // Actualizar tiempo de cooldown
        timeSinceLastAbility = Time.time;
    }

    // Sarge: Hacer invulnerable por 5 segundos
    private void SargeSA () {
            animator.SetBool("takeDamage", true);
            StartCoroutine(invincibleSarge());
            StartCoroutine(recordAnimatorHitFrames());
            timeSinceLastAbility = Time.time;

    }

    // Función de conteo de invulnerabilidad para la habilidad especial de Sarge
    // DEV: Reformular para no usar sargeActive, sino isInvulnerable
    IEnumerator invincibleSarge()
    {
        StopCoroutine(recordInvulnerabiltyFrames());
        this.sargeActive = true;
        bubble.GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(5);
        this.sargeActive = false;
        bubble.GetComponent<SpriteRenderer>().enabled = false;
    }

    // Función pública para hacer daño al jugador
    public override void GetHit(){
        // Si es invulnerable o no es propietario de este jugador, ignorar
        if (!IsOwner || isInvulnerable || this.sargeActive) {
            return;
        }

        // Hacer daño y dar invulnerabilidad o morir
        currentHealth -= 1;
        if (currentHealth <= 0) {
            Die();
            if (Gamepad.current != null && Gamepad.current.deviceId == deviceID){
                Gamepad.current.SetMotorSpeeds(0.10f, 0.25f);
                StartCoroutine(handleRumble(1.5f));
            }
        } else {
            animator.SetBool("takeDamage", true);
            StopCoroutine(recordInvulnerabiltyFrames());
            StartCoroutine(recordInvulnerabiltyFrames());
            StartCoroutine(recordAnimatorHitFrames());
            if (Gamepad.current != null && Gamepad.current.deviceId == deviceID){
                Gamepad.current.SetMotorSpeeds(0.10f, 0.25f);
                StartCoroutine(handleRumble(0.5f));
            }
        }
    }

    IEnumerator handleRumble(float time){
        yield return new WaitForSeconds(time);
        Gamepad.current.SetMotorSpeeds(0f, 0f);
    }

    // Función para morir
    private void Die(){
        // Deshabilitar controles
        enableControl = false;

        // Cambiar a estado de muerte
        gameObject.tag = "Dead Player";
        ChangeDeadStateServerRpc(true, playerNumber);
        animator.SetBool("dead", true);

        // Deshabilitar Rigidbody
        gameObject.GetComponent<Rigidbody2D>().simulated = false;

        // Eliminar movimiento
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(0f,0f,0f);

        // Decirle al server que un jugador murio
        gameManager.PlayerDiedServerRpc();
    }

    // Función de conteo de invulnerabilidad
    // Hace al jugador invulnerable por invulnerabilityWindow segundos
    IEnumerator recordInvulnerabiltyFrames()
    {
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        SpriteRenderer bSquareRenderer = this.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        renderer.color = new Color(1f, 1f, 1f, 0.5f);
        bSquareRenderer.color = new Color(1f, 1f, 1f, 0.5f);
        this.isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityWindow);
        this.isInvulnerable = false;
        renderer.color = new Color(1f, 1f, 1f, 1f);
        bSquareRenderer.color = new Color(1f, 1f, 1f, 1f);
    }

    // Función de espera para la animación de invulnerabilidad
    IEnumerator recordAnimatorHitFrames()
    {
        yield return new WaitForSeconds(0.16f);
        animator.SetBool("takeDamage", false);
    }

    // Función para respawnear al jugador
    public override void Respawn(){
        // Reiniciar estadísticas para reaplicar sponsors
        ResetStats();
        GetComponent<ItemManager>().applyItems();

        // Reiniciar animador
        animator.SetBool("dead", false);
        
        // Reiniciar tag
        gameObject.tag = "Player";

        // Reiniciar vida
        currentHealth = maxHealth;

        // Devolver control
        enableControl = true;

        // Informar a la red el nuevo estado del jugador
        ChangeDeadStateServerRpc(false, playerNumber);

        // Dar periodo de invulnerabilidad
        StopCoroutine(recordInvulnerabiltyFrames());
        StartCoroutine(recordInvulnerabiltyFrames());

        // Ir a posición inicial
        gameObject.transform.position = spawnPositions[Convert.ToInt32(playerNumber)];

        // Reiniciar rigidbody
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(0f,0f,0f);
        gameObject.GetComponent<Rigidbody2D>().simulated = true;

        // Funciones de red
        if (IsServer) {
            RespawnServerRpc();
        }
    }

    // Función que reinicia las stats del jugador (para aplicar los items)
    private void ResetStats(){
        gameObject.transform.localScale = new Vector3 (0.5f,0.5f,0.5f);
        aiPriority = 1f;
        playerSpeed = char_playerSpeed;
        bulletSpeed = char_bulletSpeed;
        maxHealth = char_maxHealth;
        fireRate = char_fireRate;
        bulletDamage = char_bulletDamage;
    }

    // Función para Despawnear
    // Usada para las fases de compra/edicion
    public override void Despawn(){
        // Enviar fuera de la vista de la cámara
        gameObject.transform.position = new Vector3(0f,100f,0f);

        // Desactivar rigidbody y controles
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(0f,0f,0f);
        gameObject.GetComponent<Rigidbody2D>().simulated = false;
        enableControl = false;
    }

    // Funciones de red
    // DEV: Reformular en un nuevo script para liberar espacio del PlayerController

    // Aparece una bala falsa local
    public override void SpawnFakeBullet(float _bulletSpeed, Vector2 _direction, ulong _playerNumber, float _x, float _y){
        if (IsOwner && playerNumber != _playerNumber) {
            GameObject clone;
            clone = Instantiate(bullethandler.GetComponent<BulletHandler>().prefabFakeBullet, new Vector3 (_x, _y, 0f), transform.rotation);
            clone.GetComponent<FakePlayerBullet>().bulletSpeed = _bulletSpeed;
            clone.GetComponent<Rigidbody2D>().velocity = (_direction) * (_bulletSpeed);
            ColorCodeToPlayer(clone, _playerNumber);
        }
    }

    // Se ejecuta en el servidor
    // Aparece un objetivo de camara que sigue al jugador
    [ServerRpc(RequireOwnership = false)]
    public void SpawnCameraTargetServerRpc(ulong _playerNumber){

    }

    // Se ejecuta en todos los clientes
    // Le dota su objetivo a la cámara
    [ClientRpc]
    public override void StartCameraClientRpc(){
        
    }

    // Se ejecuta en el servidor
    // Aparece un MenuManager para un jugador
    [ServerRpc(RequireOwnership = false)]
    public void SpawnMenuManagerServerRpc(ulong _playerNumber){
        GameObject spawnMM;
        spawnMM = Instantiate(prefabMenuManager, new Vector3(0f,0f,0f), transform.rotation);
        DontDestroyOnLoad(spawnMM);
        spawnMM.GetComponent<NetworkObject>().Spawn();
        spawnMM.GetComponent<MenuManager>().myPlayer = this.gameObject;
    }

    // Se ejecuta en el servidor
    // Cambia el estado del jugador a muerto o vivo
    // DEV: Reformulable para simplificar la lógica
    [ServerRpc(RequireOwnership = false)]
    private void ChangeDeadStateServerRpc(bool isDead, ulong _playerNumber){
        GameObject[] players;
        GameObject[] deadPlayers;
        players = GameObject.FindGameObjectsWithTag("Player");
        deadPlayers = GameObject.FindGameObjectsWithTag("Dead Player");
        // Buscar al jugador entre los jugadores vivos y actualizar su tag
        foreach (GameObject player in players) {
            if (player.GetComponent<LocalPlayerController>().playerNumber == _playerNumber) {
                if (isDead) {
                    player.tag = "Dead Player";
                } else {
                    player.tag = "Player";
                }
            }
        }
        // Buscar al jugador entre los jugadores muertos y actualizar su tag
        foreach (GameObject deadplayer in deadPlayers) {
            if (deadplayer.GetComponent<LocalPlayerController>().playerNumber == _playerNumber) {
                if (isDead) {
                    deadplayer.tag = "Dead Player";
                } else {
                    deadplayer.tag = "Player";
                }
            }
        }
        
    }

    // Reaparecer jugadores en el host
    [ServerRpc]
    private void RespawnServerRpc(){
        // Cambiar tag de los jugadores muertos
        GameObject[] deadPlayers;
        deadPlayers = GameObject.FindGameObjectsWithTag("Dead Player");
        foreach (GameObject deadplayer in deadPlayers) {
            deadplayer.tag = "Player";
        }

        // Reiniciar colisiones y animadores en host
        GameObject[] players;
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            player.gameObject.GetComponent<Animator>().SetBool("dead", false);
            gameObject.GetComponent<Rigidbody2D>().simulated = true;
        }
        
    }

    // Añadir jugador a la lista del GameManager
    [ServerRpc(RequireOwnership = false)]
    private void AddPlayerServerRpc(string _name){
        Debug.Log("rpc");
        gameManager.AddPlayer(_name);
    }

    // Cambiar personaje del jugador
    // DEV: No, no estamos orgullosos de esta solución, pero se reformulará
    public override async Task ChangeCharacter(string _characterCode){
        characterCode.Value = _characterCode;
        if (_characterCode == "cheeseman") {
            animator.runtimeAnimatorController = characterAnimators[0];
            char_playerSpeed = 8f;
            char_bulletSpeed = 30f;
            char_maxHealth = 6;
            char_fireRate = 3; // en disparos por segundo
            char_bulletDamage = 3;
            abilityCooldown = 5;
            specAb = new specialAbility(CheesemanSA);
            changeAnimatorServerRpc(playerNumber, 0);
            await Task.Yield();
        }
        if (_characterCode == "sarge") {
            animator.runtimeAnimatorController = characterAnimators[1];
            char_playerSpeed = 6f;
            char_bulletSpeed = 30f;
            char_maxHealth = 10;
            char_fireRate = 2; // en disparos por segundo
            char_bulletDamage = 4;
            abilityCooldown = 15;
            specAb = new specialAbility(SargeSA);
            changeAnimatorServerRpc(playerNumber, 1);
            await Task.Yield();
        }
    }

    // Llamar a cambiar el animador de un jugador en los clientes
    [ServerRpc(RequireOwnership = false)]
    public void changeAnimatorServerRpc(ulong _playerNumber, int _characterAnimatorNumber){
        changeAnimatorClientRpc(_playerNumber, _characterAnimatorNumber);
    }

    // Cambiar el animador de un jugador en todos los clientes
    [ClientRpc]
    public void changeAnimatorClientRpc(ulong _playerNumber, int _characterAnimatorNumber){
        GameObject[] players;
        LocalPlayerController script;
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            script = player.GetComponent<LocalPlayerController>();
            if (script.playerNumber == _playerNumber) {
                script.animator.runtimeAnimatorController = characterAnimators[_characterAnimatorNumber];
            }
        }
        
    }
}
