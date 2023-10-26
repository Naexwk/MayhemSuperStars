
using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.Collections;
using System.Threading.Tasks;

delegate void specialAbility();
public class PlayerController : NetworkBehaviour
{
    // Estadísticas del personaje
    private float char_playerSpeed = 8f;
    private float char_bulletSpeed = 30f;
    private int char_maxHealth = 6;
    private int char_fireRate = 3; // en disparos por segundo
    private int char_bulletDamage = 3;

    // Estadísticas de jugador actuales
    public float playerSpeed;
    public float bulletSpeed;
    public int maxHealth;
    public int fireRate; // en disparos por segundo
    public int bulletDamage;

    public float aiPriority = 1;

    //Animacion
    [SerializeField] private RuntimeAnimatorController[] characterAnimators;
    [SerializeField] private Animator animator;

    // Variables de control
    private bool enableControl = false;
    public float currentHealth;
    private float timeSinceLastFire;
    public float abilityCooldown; // en segundos
    public float timeSinceLastAbility;
    public int abilityDamage;
    private bool isInvulnerable;
    [SerializeField] private float invulnerabilityWindow;
    public bool sargeActive = false;

    // Variables de personaje
    public GameObject bubble;
    public NetworkVariable<FixedString64Bytes> characterCode = new NetworkVariable<FixedString64Bytes>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    // public string characterCode = "cheeseman";
    specialAbility specAb;

    // Objetos para movimiento
    private Rigidbody2D rig;

    // Objetos de cámara
    private Camera mainCamera;
    [SerializeField] private GameObject cameraTargetPrefab;

    // Objetos de Network
    public ulong playerNumber;
    private GameObject bullethandler;
    [SerializeField] private GameObject prefabMenuManager;

    // Variables visuales
    private GameObject outline;
    
    // Spawn points
    // DEV: Hacer GameObjects para modificarlos en escena
    private Vector3[] spawnPositions = { new Vector3(56.5f,28f,0f), new Vector3(57.5f,14.5f,0f), new Vector3(29f,13.5f,0f), new Vector3(23.5f,30f,0f) };
    //private Vector3[] spawnPositions = { new Vector3(65.83f,36.37f,0f), new Vector3(67f,22.5f,0f), new Vector3(38.24f,21.71f,0f), new Vector3(32.7f,38.65f,0f) };

    // Función para colorear objetos según el número del jugador
    void ColorCodeToPlayer (GameObject go, ulong playerNumber) {
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

    // Conseguir numero de jugador
    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        DontDestroyOnLoad(this.gameObject);
        playerNumber = gameObject.GetComponent<NetworkObject>().OwnerClientId;
        if (IsOwner) {
            characterCode.Value = "cheeseman";
        }
    }

    // Inicializar controladores de jugador
    async void Start()
    {
        //bubble = transform.GetChild(0).gameObject;
        bubble.GetComponent<SpriteRenderer>().enabled = false;
        rig = gameObject.GetComponent<Rigidbody2D>();
        GameObject gameManager = GameObject.FindWithTag("GameManager");
        mainCamera = Camera.main;
        playerNumber = gameObject.GetComponent<NetworkObject>().OwnerClientId;
        if (IsOwner){
            // Añadir nombre del jugador al server
            GameObject relayManager = GameObject.FindWithTag("RelayManager");
            string name = relayManager.GetComponent<LanBehaviour>().playerName;
            AddPlayerServerRpc(name);
        }

        // DEV: Reañadir outline
        outline = gameObject.transform.GetChild(0).gameObject;
        ColorCodeToPlayer(outline, playerNumber);
        if (IsOwner) {
            // Inicializar como personaje default (cheeseman)
            await ChangeCharacter("cheeseman");
        }

        // Recibir personajes de los jugadores que ya están en escena
        GameObject[] players;
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            PlayerController script = player.GetComponent<PlayerController>();
            if (script.playerNumber != this.playerNumber) {
                await script.ChangeCharacter(script.characterCode.Value.ToString());
            }
        }
    }

    // Escuchar cambios de escena
    private void Awake() {
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameManager.state.OnValueChanged += StateChange;
    }

    // Ejecutar funciones de escena de juego
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SampleScene") {
            mainCamera = Camera.main;
            bullethandler = GameObject.FindWithTag("BulletHandler");
            if (IsOwner) {
                SpawnCameraTargetServerRpc(playerNumber);
                SpawnMenuManagerServerRpc(playerNumber);
            }
            
        }
    }

    // Función de cambio de estado de juego
    private void StateChange(GameState prev, GameState curr){
        // Si empieza una ronda de juego, permitir al jugador usar su habilidad especial
        if (this != null) {
            if (curr == GameState.Round || curr == GameState.StartGame) {
                timeSinceLastAbility = Time.time - abilityCooldown;
            }
        }
    }

    void Update()
    {
        // Si no es dueño de este script o no está habilitado
        // el  control, ignorar
        if (!IsOwner || !enableControl) {
            return;
        }

        // Disparar con el clic izquierdo
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Shoot();
        }

        // Usar habilidad especial con el clic derecho
        if (Input.GetKey(KeyCode.Mouse1))
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
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");

        // Mover según el input, normalizado
        if (xInput == 0 || yInput == 0) {
            rig.velocity = new Vector2(xInput * playerSpeed, yInput * playerSpeed);
        } else {
            rig.velocity = new Vector2(xInput * playerSpeed * 0.707f, yInput * playerSpeed* 0.707f);
        }

        // Añadir velocidad al animador según la velocidad
        animator.SetFloat("Speed", Mathf.Abs(rig.velocity.magnitude));
    } 

    // Dispara una bala si ya se cumplió el tiempo de espera del firerate.
    private void Shoot(){
        if ((Time.time - timeSinceLastFire) > (1f/fireRate)) {
            // Obtener dirección de disparo
            Vector3 worldMousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = worldMousePos - transform.position;
            direction.Normalize();

            // Disparar en red
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
        // Encontrar dirección de disparo
        Vector3 worldMousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = worldMousePos - transform.position;
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
    public void GetHit(){
        // Si es invulnerable o no es propietario de este jugador, ignorar
        if (!IsOwner || isInvulnerable || this.sargeActive) {
            return;
        }

        // Hacer daño y dar invulnerabilidad o morir
        currentHealth -= 1;
        if (currentHealth <= 0) {
            Die();
        } else {
            animator.SetBool("takeDamage", true);
            StartCoroutine(recordInvulnerabiltyFrames());
            StartCoroutine(recordAnimatorHitFrames());
        }
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
    public void Respawn(){
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
    public void Despawn(){
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
    public void SpawnFakeBullet(float _bulletSpeed, Vector2 _direction, ulong _playerNumber, float _x, float _y){
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
        GameObject spawnCam;
        spawnCam = Instantiate(cameraTargetPrefab, new Vector3(0f,0f,0f), transform.rotation);
        spawnCam.GetComponent<NetworkObject>().SpawnWithOwnership(_playerNumber);
    }

    // Se ejecuta en todos los clientes
    // Le dota su objetivo a la cámara
    [ClientRpc]
    public void StartCameraClientRpc(){
        GameObject mainCam;
        mainCam = GameObject.FindWithTag("MainCamera");
        GameObject[] cameraTargets = GameObject.FindGameObjectsWithTag("CameraTarget");
        foreach (GameObject cameraTarget in cameraTargets)
        {
            if(cameraTarget.GetComponent<NetworkObject>().IsOwner){
                mainCam.GetComponent<CameraMovement>().SetCameraTarget(cameraTarget.transform);
                cameraTarget.GetComponent<CameraTarget>().StartCam();
            }
        }
        
    }

    // Se ejecuta en el servidor
    // Aparece un MenuManager para un jugador
    [ServerRpc(RequireOwnership = false)]
    public void SpawnMenuManagerServerRpc(ulong _playerNumber){
        GameObject spawnMM;
        spawnMM = Instantiate(prefabMenuManager, new Vector3(0f,0f,0f), transform.rotation);
        DontDestroyOnLoad(spawnMM);
        spawnMM.GetComponent<NetworkObject>().SpawnWithOwnership(_playerNumber);
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
            if (player.GetComponent<PlayerController>().playerNumber == _playerNumber) {
                if (isDead) {
                    player.tag = "Dead Player";
                } else {
                    player.tag = "Player";
                }
            }
        }
        // Buscar al jugador entre los jugadores muertos y actualizar su tag
        foreach (GameObject deadplayer in deadPlayers) {
            if (deadplayer.GetComponent<PlayerController>().playerNumber == _playerNumber) {
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
        GameObject gameManager = GameObject.FindWithTag("GameManager");
        gameManager.GetComponent<GameManager>().AddPlayer(_name);
    }

    // Cambiar personaje del jugador
    // DEV: No, no estamos orgullosos de esta solución, pero se reformulará
    public async Task ChangeCharacter(string _characterCode){
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
        PlayerController script;
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            script = player.GetComponent<PlayerController>();
            if (script.playerNumber == _playerNumber) {
                script.animator.runtimeAnimatorController = characterAnimators[_characterAnimatorNumber];
            }
        }
        
    }
}
