using System.Collections;
using System.Collections.Generic;
using UnityEngine;

delegate void gameItem();

// Controlador de sponsors del jugador
public class ItemManager : MonoBehaviour
{  
    // Array de funciones de sponsors
    gameItem[] itemInventory = new gameItem[10];

    // Número de objetos obtenidos en índice -1.
    private int obtainedItemsNumber = -1;

    // Aplicar los efectos de todos los sponsors del jugador
    // Las estadísticas del jugador deberían resetearse
    // y los sponsors deberían aplicarse cada ronda.
    public void applyItems () {
        for (int i = 0; i <= obtainedItemsNumber; i++) {
            itemInventory[i]();
        }
    }

    // Función para añadir lógica de objeto
    // DEV: Será mejor reformular esto
    public void addItem(int itemID) {
        obtainedItemsNumber++;
        if (itemID == 1) {
            itemInventory[obtainedItemsNumber] = new gameItem(SausageHeart);
            return;
        }

        if (itemID == 2) {
            itemInventory[obtainedItemsNumber] = new gameItem(Fosfofosfo);
            return;
        }

        if (itemID == 3) {
            itemInventory[obtainedItemsNumber] = new gameItem(Cheese);
            return;
        }
    }

    // Funciones de items

    // Sausage Heart: Añade un corazón
    void SausageHeart () {
        gameObject.GetComponent<PlayerController>().maxHealth += 2;
        gameObject.GetComponent<PlayerController>().currentHealth += 2;
    }

    // Fosfofosfo: Añade velocidad
    void Fosfofosfo () {
        gameObject.GetComponent<PlayerController>().playerSpeed += 1;
    }

    // Cheese: Añade daño
    void Cheese () {
        gameObject.GetComponent<PlayerController>().bulletDamage += 1;
    }
}
