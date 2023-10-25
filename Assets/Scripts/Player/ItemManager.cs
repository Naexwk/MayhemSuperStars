using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;

delegate void gameItem();

// Controlador de sponsors del jugador
public class ItemManager : NetworkBehaviour
{  
    [SerializeField] private GameObject codersSmellParticles;
    // Array de funciones de sponsors
    gameItem[] allItems = new gameItem[7];
    // Array de sponsors de jugador
    gameItem[] itemInventory = new gameItem[10];

    // Número de objetos obtenidos en índice -1.
    private int obtainedItemsNumber = -1;

    private void Awake() {
        allItems[0] = new gameItem(SausageHeart);
        allItems[1] = new gameItem(Fosfofosfo);
        allItems[2] = new gameItem(Cheese);
        allItems[3] = new gameItem(Testosterone);
        allItems[4] = new gameItem(RugileoPepsi);
        allItems[5] = new gameItem(CodersSmell);
        allItems[6] = new gameItem(Vampire);
    }

    // Aplicar los efectos de todos los sponsors del jugador
    // Las estadísticas del jugador deberían resetearse
    // y los sponsors deberían aplicarse cada ronda.
    public void applyItems () {
        if (IsOwner) {
            for (int i = 0; i <= obtainedItemsNumber; i++) {
                itemInventory[i]();
            }
        }
        
    }

    // Función para añadir lógica de objeto
    public void addItem(int itemID) {
        obtainedItemsNumber++;
        itemInventory[obtainedItemsNumber] = allItems[itemID-1];
    }

    // Funciones de items

    // (1) Sausage Heart: Añade un corazón
    void SausageHeart () {
        gameObject.GetComponent<PlayerController>().maxHealth += 2;
        gameObject.GetComponent<PlayerController>().currentHealth += 2;
    }

    // (2) Fosfofosfo: Añade velocidad
    void Fosfofosfo () {
        gameObject.GetComponent<PlayerController>().playerSpeed += 1;
    }

    // (3) Cheese: Añade daño
    void Cheese () {
        gameObject.GetComponent<PlayerController>().bulletDamage += 1;
    }

    // (4) Testosterone: Mejora el fireRate
    void Testosterone () {
        gameObject.GetComponent<PlayerController>().fireRate += 1;
    }

    // (5) Rugileo y Pepsi: Hace crecer al jugador, aumenta su daño y vida
    void RugileoPepsi () {
        gameObject.transform.localScale += new Vector3 (0.3f,0.3f,0.3f);
        Camera.main.orthographicSize += 1;
        gameObject.GetComponent<PlayerController>().bulletDamage += 1;
        gameObject.GetComponent<PlayerController>().maxHealth += 2;
        gameObject.GetComponent<PlayerController>().currentHealth += 2;
        if (IsOwner) {
            RugileoPepsiHelperServerRpc(gameObject.transform.localScale.x,
            gameObject.transform.localScale.y, gameObject.transform.localScale.z);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RugileoPepsiHelperServerRpc(float x, float y, float z){
        gameObject.transform.localScale = new Vector3 (x,y,z);
        RugileoPepsiHelperClientRpc(x,y,z);
    }

    [ClientRpc]
    public void RugileoPepsiHelperClientRpc(float x, float y, float z){
        gameObject.transform.localScale = new Vector3 (x,y,z);
    }

    // (6) Coder's Smell: Hace que los enemigos eviten al jugador
    void CodersSmell () {
        gameObject.GetComponent<PlayerController>().aiPriority -= 0.3f;
        GameObject smell;
        Quaternion direction = Quaternion.Euler(-90, 0, 0);
        smell = Instantiate(codersSmellParticles, transform.position, direction);
        smell.transform.parent = gameObject.transform;
    }

    // (7) Vampire: Cada que golpeas a un enemigo, hay una probabilidad de recuperar vida
    void Vampire () {
        // No hace nada chsm
    }


}
