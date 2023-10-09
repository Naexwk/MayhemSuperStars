// This script manages the prefabs and buttons so editables can be placed, it also instantiates the prefabs in the network
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class LevelEditorManager : NetworkBehaviour
{
    public ItemController itemController; 
    public GameObject[] ItemPrefabs;
    public int CurrentButtonPressed; //button ID, can be swapped with the ID for the object depending on how we decide to call this

    public void spawnEditable(){
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        // checks whetther object can be placed and spawns it them
        if(itemController.tempObject != null){
            EditModeSpawnerServerRpc(worldPosition.x, worldPosition.y, CurrentButtonPressed);
        }
    }

    [ServerRpc (RequireOwnership=false)] //thus functions instatie the objects in the netwrk depending on wether the user is clien or host
    void EditModeSpawnerServerRpc(float x, float y, int index){
        GameObject spawnedObject;
        spawnedObject = Instantiate(ItemPrefabs[index], new Vector3(x, y, 0), Quaternion.identity);
        if (spawnedObject.tag == "Spawner") {
            spawnedObject.GetComponent<NetworkObject>().Spawn();
        } else {
            EditModeSpawnerClientRpc(x, y, index);
        }
        
    }

    [ClientRpc]
    void EditModeSpawnerClientRpc(float x, float y, int index){
        if(!IsServer){
            Instantiate(ItemPrefabs[index], new Vector3(x, y, 0), Quaternion.identity);
        }
    }
}
