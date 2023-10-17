
using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

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

    //Animacion
    public RuntimeAnimatorController[] characterAnimators;
    public Animator animator;

    // Variables de control
    public bool enableControl = false;
    public float currentHealth;
    private float timeSinceLastFire;
    public float abilityCooldown; // en segundos
    private float timeSinceLastAbility;
    public int abilityDamage;
    public bool isInvulnerable;
    public float invulnerabilityWindow;
    public bool sargeActive = false;
    public int points = 0;

    // Variables de personaje
    public string characterCode = "cheeseman";
    public GameObject bubble;
    specialAbility specAb;

    // Objetos para movimiento
    public Rigidbody2D rig;

    // Objetos de cámara
    private Camera _mainCamera;
    public GameObject cameraTargetPrefab;

    // Objetos de Network
    public ulong playerNumber;
    private GameObject bullethandler;
    public GameObject prefabMenuManager;

    // Variables visuales
    private GameObject outline;
    
    // Spawn points
    // hardcodeado porque unity me odia
    private Vector3[] spawnPositions = { new Vector3(15f,4.5f,0f), new Vector3(16f,-9.23f,0f), new Vector3(-12.5f,-10f,0f), new Vector3(-18f,6.85f,0f) };

    // Función para colorear objetos según el número del jugador
    void colorCodeToPlayer (GameObject go, ulong playerNumber) {
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

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        DontDestroyOnLoad(this.gameObject);
        playerNumber = gameObject.GetComponent<NetworkObject>().OwnerClientId;
        //if (IsOwner){
            //Debug.Log("player number: " + playerNumber);
            //spawnMenuManagerServerRpc(playerNumber);
        //}
    }

    // Obtener su cámara, generador de balas, número de jugador y colorear su outline
    // También asigna la función de habilidad especial a specAb
    void Start()
    {
        //bubble = transform.GetChild(0).gameObject;
        bubble.GetComponent<SpriteRenderer>().enabled = false;
        GameObject gameManager = GameObject.FindWithTag("GameManager");
        //gameManager.GetComponent<GameManager>().AddPlayer(name);
        _mainCamera = Camera.main;
        bullethandler = GameObject.FindWithTag("BulletHandler");
        playerNumber = gameObject.GetComponent<NetworkObject>().OwnerClientId;
        outline = gameObject.transform.GetChild(0).gameObject;

        //Instantiate(prefabMenuManager, new Vector3(0f,0f,0f), transform.rotation);
        if (IsOwner){
            spawnMenuManagerServerRpc(playerNumber);
        }
        
        
        colorCodeToPlayer(outline, playerNumber);
        if (characterCode == "cheeseman") {
            specAb = new specialAbility(CheesemanSA);
        }
        if (IsOwner) {
            GameObject relayManager = GameObject.FindWithTag("RelayManager");
            string name = relayManager.GetComponent<LanBehaviour>().playerName;
            addPlayerServerRpc(name);
        }

        changeCharacter("cheeseman");
        //colorCodeToPlayer(outline, playerNumber);
    }

    private void Awake() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SampleScene") {
            _mainCamera = Camera.main;
            bullethandler = GameObject.FindWithTag("BulletHandler");
            
            outline = gameObject.transform.GetChild(0).gameObject;

            //Instantiate(prefabMenuManager, new Vector3(0f,0f,0f), transform.rotation);
            
            // DEV 
            

            if (IsOwner) {
                spawnCameraTargetServerRpc(playerNumber);
                spawnMenuManagerServerRpc(playerNumber);
            }
        }
    }

    void Update()
    {
        // Si no es dueño de este script, ignorar
        if (!IsOwner) {
            return;
        }

        // Si no está habilitado el control, ignorar
        if (!enableControl){
            return;
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Shoot();
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            castSpecialAbility();
        }
    }

    private void FixedUpdate(){

        // Si no es dueño de este script, ignorar
        if (!IsOwner) {
            return;
        }

        // Si no está habilitado el control, ignorar
        if (!enableControl){
            return;
        }

        Move();
        
    }

    // Maneja el movimiento del jugador
    private void Move(){
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");

        //Debug.Log("X: " + xInput + " - Y: " + yInput);

        if (xInput == 0 || yInput == 0) {
            rig.velocity = new Vector2(xInput * playerSpeed, yInput * playerSpeed);
        } else {
            rig.velocity = new Vector2(xInput * playerSpeed * 0.707f, yInput * playerSpeed* 0.707f);
        }

        animator.SetFloat("Speed", Mathf.Abs(rig.velocity.magnitude));
    } 

    // Dispara una bala si ya se cumplió el tiempo de espera del firerate.
    private void Shoot(){
        if ((Time.time - timeSinceLastFire) > (1f/fireRate)) {
            Vector3 worldMousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = worldMousePos - transform.position;
            direction.Normalize();
            bullethandler.GetComponent<BulletHandler>().spawnBulletServerRpc(bulletSpeed, direction, playerNumber, transform.position.x, transform.position.y);
            timeSinceLastFire = Time.time;

            GameObject clone;
            clone = Instantiate(bullethandler.GetComponent<BulletHandler>().prefabBullet, transform.position, transform.rotation);
            clone.GetComponent<PlayerBullet>().bulletDamage = bulletDamage;
            clone.GetComponent<PlayerBullet>().bulletSpeed = bulletSpeed;
            clone.GetComponent<PlayerBullet>().bulletDirection = direction;
            clone.GetComponent<Rigidbody2D>().velocity = (direction) * (bulletSpeed);
            colorCodeToPlayer(clone, playerNumber);
        }
    }

    // Castea la habilidad especial, que depende del personaje
    private void castSpecialAbility(){
        if ((Time.time - timeSinceLastAbility) > (abilityCooldown)) {
            specAb();
        }
    }

    // Funciones de habilidades especiales

    // Cheeseman: Aparece una bola de queso que daña a los enemigos
    private void CheesemanSA () {
        
            Vector3 worldMousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = worldMousePos - transform.position;
            direction.Normalize();
            timeSinceLastAbility = Time.time;

            // DEV: Esto es para crear una bala client-side. Es muy poco confiable, entonces opté por
            // una bala server-side, aunque se vea con un poco de lag.
            /*if (!IsServer){
                GameObject clone;
                clone = Instantiate(prefabCheeseBullet, transform.position, transform.rotation);
                Physics2D.IgnoreCollision(clone.transform.GetComponent<Collider2D>(), GetComponent<Collider2D>());
                clone.GetComponent<CheeseBullet>().bulletDamage = 3;
                clone.GetComponent<Rigidbody2D>().velocity = (direction) * (10f);
            }*/

            bullethandler.GetComponent<BulletHandler>().spawnCheeseBulletServerRpc(direction, playerNumber, abilityDamage, transform.position.x, transform.position.y);
    }

    private void SargeSA () {
        
            
            //animator.SetBool("takeDamage", true);
            //this.sargeActive = true;
            //this.isInvulnerable = true;
            //StopCoroutine(recordInvulnerabiltyFrames());
            StartCoroutine(invincibleSarge());
            //StartCoroutine(recordAnimatorHitFrames());
            //this.sargeActive = false;
            timeSinceLastAbility = Time.time;

    }

    IEnumerator invincibleSarge()
    {
        //this.isInvulnerable = true;
        StopCoroutine(recordInvulnerabiltyFrames());
        this.sargeActive = true;
        bubble.GetComponent<SpriteRenderer>().enabled = true;
        //this.isInvulnerable = true;
        Debug.Log("Invulnerable");
        yield return new WaitForSeconds(5);
        //this.isInvulnerable = false;
        this.sargeActive = false;
        bubble.GetComponent<SpriteRenderer>().enabled = false;
        Debug.Log("Vulnerable");
    }

    // Función pública para hacer daño al jugador
    public void GetHit(){
        if (!IsOwner) {
            return;
        }

        // Si es invulnerable, ignorar
        if (this.isInvulnerable || this.sargeActive) {
            return;
        }

        // Hacer daño y dar invulnerabilidad o morir
        currentHealth -= 1;
        if (currentHealth <= 0) {
            Die();
        } else {
            animator.SetBool("takeDamage", true);
            Debug.Log("Got hit");
            StartCoroutine(recordInvulnerabiltyFrames());
            Debug.Log("hitted");
            StartCoroutine(recordAnimatorHitFrames());
        }
    }

    // El jugador cae de lado, y se le quita el control
    public void Die(){
        
        //transform.rotation = Quaternion.Euler(new Vector3(0,0,90));
        enableControl = false;
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(0f,0f,0f);
        gameObject.tag = "Dead Player";
        gameObject.GetComponent<Rigidbody2D>().simulated = false;
        changeDeadStateServerRpc(true, playerNumber);
        animator.SetBool("dead", true);
    }

    // Se miden unos segundos igual a invulnerabilityWindow,
    // durante este tiempo el jugador es transparente e invulnerable
    IEnumerator recordInvulnerabiltyFrames()
    {
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        SpriteRenderer bSquareRenderer = this.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        renderer.color = new Color(1f, 1f, 1f, 0.5f);
        bSquareRenderer.color = new Color(1f, 1f, 1f, 0.5f);
        this.isInvulnerable = true;
        /*
        if (this.sargeActive){
            Debug.Log("Sarge Activado");
            yield break;
        }
        */
        yield return new WaitForSeconds(invulnerabilityWindow);
        this.isInvulnerable = false;
        renderer.color = new Color(1f, 1f, 1f, 1f);
        bSquareRenderer.color = new Color(1f, 1f, 1f, 1f);
    }

    IEnumerator recordAnimatorHitFrames()
    {
        yield return new WaitForSeconds(0.16f);
        animator.SetBool("takeDamage", false);
    }

    public void givePoints(int _points){
        points += _points;
    }


    // Ignora las colisiones con otros jugadores
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Dead Player")
        {
            Physics2D.IgnoreCollision(collision.transform.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }

    // Función para respawnear al jugador
    // DEV: Añadir reset de estadísticas
    public void Respawn(){
        ResetStats();
        animator.SetBool("dead", false);
        GetComponent<ItemManager>().applyItems();
        gameObject.tag = "Player";
        currentHealth = maxHealth;
        enableControl = true;
        changeDeadStateServerRpc(false, playerNumber);
        StartCoroutine(recordInvulnerabiltyFrames());
        transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
        gameObject.transform.position = spawnPositions[Convert.ToInt32(playerNumber)];
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(0f,0f,0f);
        gameObject.GetComponent<Rigidbody2D>().simulated = true;
        if (IsServer) {
            RespawnServerRpc();
        }
        //spawnPlayerClientRpc();
    }

    // Función que reinicia las stats del jugador (para aplicar los items)
    public void ResetStats(){
        playerSpeed = char_playerSpeed;
        bulletSpeed = char_bulletSpeed;
        maxHealth = char_maxHealth;
        fireRate = char_fireRate;
        bulletDamage = char_bulletDamage;
    }

    // Función para Despawnear
    // Usada para las fases de compra/edicion
    public void Despawn(){
        gameObject.transform.position = new Vector3(0f,50f,0f);
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(0f,0f,0f);
        gameObject.GetComponent<Rigidbody2D>().simulated = false;
        enableControl = false;

        // DEV: Move to the Shadow Realm
    }

    public void spawnFakeBullet(float _bulletSpeed, Vector2 _direction, ulong _playerNumber, float _x, float _y){
        if (IsOwner && playerNumber != _playerNumber) {
            GameObject clone;
            clone = Instantiate(bullethandler.GetComponent<BulletHandler>().prefabFakeBullet, new Vector3 (_x, _y, 0f), transform.rotation);
            clone.GetComponent<FakePlayerBullet>().bulletSpeed = _bulletSpeed;
            clone.GetComponent<Rigidbody2D>().velocity = (_direction) * (_bulletSpeed);
            colorCodeToPlayer(clone, _playerNumber);
        }
    }

    // Se ejecuta en el servidor
    // Aparece un target que sigue al jugador
    [ServerRpc(RequireOwnership = false)]
    public void spawnCameraTargetServerRpc(ulong _playerNumber){
        //Debug.Log("Called from " + _playerNumber);
        GameObject spawnCam;
        spawnCam = Instantiate(cameraTargetPrefab, new Vector3(0f,0f,0f), transform.rotation);
        spawnCam.GetComponent<NetworkObject>().SpawnWithOwnership(_playerNumber);
        
    }

    // Se ejecuta en todos los clientes
    // Le dota su objetivo a la cámara
    [ClientRpc]
    public void startCameraClientRpc(){
        //Debug.Log("Called here");
        GameObject mainCam;
        mainCam = GameObject.FindWithTag("MainCamera");
        
        
        GameObject[] cameraTargets = GameObject.FindGameObjectsWithTag("CameraTarget");
        foreach (GameObject cameraTarget in cameraTargets)
        {
            if(cameraTarget.GetComponent<NetworkObject>().IsOwner){
                mainCam.GetComponent<CameraMovement>().setCameraTarget(cameraTarget.transform);
                cameraTarget.GetComponent<CameraTarget>().StartCam();
            }
        }
        
    }

    
    [ServerRpc(RequireOwnership = false)]
    public void spawnMenuManagerServerRpc(ulong _playerNumber){
        //Debug.Log("Called from " + _playerNumber);
        GameObject spawnMM;
        spawnMM = Instantiate(prefabMenuManager, new Vector3(0f,0f,0f), transform.rotation);
        DontDestroyOnLoad(spawnMM);
        spawnMM.GetComponent<NetworkObject>().SpawnWithOwnership(_playerNumber);
        
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void changeDeadStateServerRpc(bool isDead, ulong _playerNumber){
        //Debug.Log("Called from " + _playerNumber + " with IsDead = " + isDead);
        GameObject[] players;
        GameObject[] deadplayers;
        players = GameObject.FindGameObjectsWithTag("Player");
        deadplayers = GameObject.FindGameObjectsWithTag("Dead Player");
        foreach (GameObject player in players) {
            if (player.GetComponent<PlayerController>().playerNumber == _playerNumber) {
                if (isDead) {
                    player.tag = "Dead Player";
                } else {
                    player.tag = "Player";
                }
            }
        }

        foreach (GameObject deadplayer in deadplayers) {
            if (deadplayer.GetComponent<PlayerController>().playerNumber == _playerNumber) {
                if (isDead) {
                    deadplayer.tag = "Dead Player";
                } else {
                    deadplayer.tag = "Player";
                }
            }
        }
        
    }

    [ServerRpc]
    public void RespawnServerRpc(){
        GameObject[] deadplayers;
        deadplayers = GameObject.FindGameObjectsWithTag("Dead Player");
        foreach (GameObject deadplayer in deadplayers) {
            deadplayer.tag = "Player";
        }
        GameObject[] players;
        players = GameObject.FindGameObjectsWithTag("Player");
        //Debug.Log(players.Length);
        foreach (GameObject player in players) {
            player.gameObject.GetComponent<Animator>().SetBool("dead", false);
            gameObject.GetComponent<Rigidbody2D>().simulated = true;
        }
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void addPlayerServerRpc(string _name){
        GameObject gameManager = GameObject.FindWithTag("GameManager");
        gameManager.GetComponent<GameManager>().AddPlayer(_name);
    }

    public void changeCharacter(string _characterCode){
        characterCode = _characterCode;
        if (_characterCode == "cheeseman") {
            animator.runtimeAnimatorController = characterAnimators[0];
            char_playerSpeed = 8f;
            char_bulletSpeed = 30f;
            char_maxHealth = 6;
            char_fireRate = 3; // en disparos por segundo
            char_bulletDamage = 3;
            abilityCooldown = 5;
            specAb = new specialAbility(CheesemanSA);
            return;
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
            return;
        }
    }




}
