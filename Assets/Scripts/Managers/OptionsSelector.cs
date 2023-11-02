using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class OptionsSelector : NetworkBehaviour
{
    bool handled = false;
    private PlayerController myPlayer;
    private int power;
    private GameManager gameManager;

    public int[][] propOptions = new int[3][];
    public int[] sponsorOptions = {1,2,3};

    public int[][] helperPropArray = new int[3][];
    public int[] helperSponsorArray;

    int[][] propDefaults = new int[3][];
    int[][] propPower_1;
    int[][] propPower_2 = new int[1][];
    int[][] propPower_3 = new int[2][];
    int[][] propPower_4;
    int[][] propPower_5;
    int[][] propPower_6 = new int[1][];

    int[] sponsorDefaults = {1,2,3}; // Sausage Heart, Fosfofosfo, cheese
    int[] sponsorPower_1 = {4}; // Testosterone
    int[] sponsorPower_2 = {}; // 
    int[] sponsorPower_3 = {5}; // Coder's smell
    int[] sponsorPower_4 = {6}; // Rugileo y Pepsi
    int[] sponsorPower_5 = {}; // 
    int[] sponsorPower_6 = {7}; // Vampire

    private void LoadProps() {
        // Cargar opciones de props
        propDefaults[0] = new int[] {0,3}; // 3 walls
        propDefaults[1] = new int[] {1,1}; // 1 Turret
        propDefaults[2] = new int[] {2,1}; // 1 Zombie Grave

        propPower_2[0] = new int[] {4, 1}; // 1 Acid Rain
        propPower_3[0] = new int[] {3,1}; // 1 Bomb
        propPower_3[1] = new int [] {5, 1}; // 1 Dropship
        propPower_6[0] = new int[] {6,1}; // 1 Catnado
    }

    private void SearchForMyPlayer(){
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<NetworkObject>().IsOwner) {
                myPlayer = player.GetComponent<PlayerController>();
            }
        }
    }

    private void Awake() {
        LoadProps();
        SearchForMyPlayer();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        // Load props
        propOptions[0] = propDefaults[0];
        propOptions[1] = propDefaults[1];
        propOptions[2] = propDefaults[2];

        GameManager.handleLeaderboard.OnValueChanged += OnLeaderboardChange;
        GameManager.state.OnValueChanged += StateChange;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Ejecutar funciones de escena de juego
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SampleScene") {
            LoadProps();
            SearchForMyPlayer();
            gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

            // Load props
            propOptions[0] = propDefaults[0];
            propOptions[1] = propDefaults[1];
            propOptions[2] = propDefaults[2];
        }
    }

    

    private void OnLeaderboardChange(bool prev, bool curr){
        if (this != null && !handled) {
            handled = true;
            GetNewOptions();
        }
    }

     private void StateChange(GameState prev, GameState curr){
        if (this != null) {
            if (curr == GameState.Round || curr == GameState.StartGame) {
                handled = false;
            }
        }
    }

    private void GetPower(){
        // Voy ganando
        if (gameManager.networkLeaderboard[0] == Convert.ToInt32(myPlayer.playerNumber)) {
            power = gameManager.currentRound - 1;
        // Voy ultimo
        } else if (gameManager.networkLeaderboard[GameManager.numberOfPlayers.Value-1] == Convert.ToInt32(myPlayer.playerNumber)) {
            power = gameManager.currentRound + 1;
        // Voy en medio
        } else {
            power = gameManager.currentRound;
        }
    }

    private void GetList(int _power){
        switch(_power){
            case 0:
                helperPropArray = propDefaults;
                helperSponsorArray = sponsorDefaults;
                break;
            case 1:
                helperPropArray = propDefaults;
                //helperPropArray = propPower_1;
                helperSponsorArray = sponsorPower_1;
                break;
            case 2:
                helperPropArray = propPower_2;
                helperSponsorArray = sponsorDefaults;
                //helperSponsorArray = sponsorPower_2;
                break;
            case 3:
                helperPropArray = propPower_3;
                helperSponsorArray = sponsorPower_3;
                break;
            case 4:
                helperPropArray = propDefaults;
                //helperPropArray = propPower_4;
                helperSponsorArray = sponsorPower_4;
                break;
            case 5:
                helperPropArray = propDefaults;
                helperSponsorArray = sponsorDefaults;
                //helperPropArray = propPower_5;
                //helperSponsorArray = sponsorPower_5;
                break;
            case 6:
                helperPropArray = propPower_6;
                helperSponsorArray = sponsorPower_6;
                break;
            default:
                helperPropArray = propDefaults;
                helperSponsorArray = sponsorDefaults;
                break;
        }
    }

    private void GetNewOptions () {

        // Reiniciar opciones
        propOptions = new int [3][];
        sponsorOptions = new int[3];

        GetPower();
        int[] lists = new int[3];

        for (int i = 0; i < 3; i++) {
            if ((power-1 + i) < 0) {
                lists[i] = 0;
            } else {
                lists[i] = (power-1+i);
            }
        }

        if ((UnityEngine.Random.Range(0, 10) + 1) > gameManager.currentRound) {
            lists[UnityEngine.Random.Range(0, 3)] = 0;
        }

        int[] usedLists = new int[3] {-1,-1,-1};
        int index = 0;

        // DEV: Eliminar ya que haya objetos de todos los poderes
        ReshuffleSponsors(sponsorDefaults);
        ReshuffleProps(propDefaults);
        bool firstTime = true;
        // Obtener sponsors
        for (int i = 0; i < lists.Length; i++) {
            if (Array.IndexOf(usedLists, lists[i]) != -1) {
                GetList(lists[i]);
                index++;
                sponsorOptions[i] = helperSponsorArray[index];
            } else {
                GetList(lists[i]);
                // DEV: Eliminar la primera parte de este if ya que haya objetos de todos los poderes
                // Dejar solo el else
                if (helperSponsorArray == sponsorDefaults) {
                    if (!firstTime) {
                        index++;
                    }
                    sponsorOptions[i] = helperSponsorArray[index];
                    usedLists[i] = 0;
                    firstTime = false;
                } else {
                    ReshuffleSponsors(helperSponsorArray);
                    sponsorOptions[i] = helperSponsorArray[0];
                    usedLists[i] = lists[i];
                }
                
            }
        }

        
        usedLists = new int[3] {-1,-1,-1};
        index = 0;

        // DEV: Eliminar ya que haya objetos de todos los poderes
        firstTime = true;

        // Obtener props
        for (int i = 0; i < lists.Length; i++) {
            if (Array.IndexOf(usedLists, lists[i]) != -1) {
                GetList(lists[i]);
                index++;
                propOptions[i] = helperPropArray[index];
            } else {
                GetList(lists[i]);
                // DEV: Eliminar la primera parte de este if ya que haya objetos de todos los poderes
                // Dejar solo el else
                if (helperPropArray == propDefaults) {
                    if (!firstTime) {
                        index++;
                    }
                    propOptions[i] = helperPropArray[index];
                    usedLists[i] = 0;
                    firstTime = false;
                } else {
                    ReshuffleProps(helperPropArray);
                    propOptions[i] = helperPropArray[0];
                    usedLists[i] = lists[i];
                }
            }
        }

        // Hardcodeado para volver a defaults
        // Eliminar ya que existan los objetos
        /*propOptions[0] = propDefaults[0];
        propOptions[1] = propDefaults[1];
        propOptions[2] = propDefaults[2];
        sponsorOptions[0] = 1;
        sponsorOptions[1] = 2;
        sponsorOptions[2] = 6;*/
    }

    void ReshuffleSponsors(int[] arraySponsors)
    {
        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int i = 0; i < arraySponsors.Length; i++ )
        {
            int tmp = arraySponsors[i];
            int r = UnityEngine.Random.Range(i, arraySponsors.Length);
            arraySponsors[i] = arraySponsors[r];
            arraySponsors[r] = tmp;
        }
    }

    void ReshuffleProps(int[][] arrayProps)
    {
        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int i = 0; i < arrayProps.Length; i++ )
        {
            int[] tmp = arrayProps[i];
            int r = UnityEngine.Random.Range(i, arrayProps.Length);
            arrayProps[i] = arrayProps[r];
            arrayProps[r] = tmp;
        }
    }
}
