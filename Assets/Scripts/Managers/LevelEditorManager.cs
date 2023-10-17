using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

// Controlador de prefabs de props para el modo edición
public class LevelEditorManager : NetworkBehaviour
{
    // Referencia al itemController, controlador de los objetos fantasmas
    [SerializeField] private ItemController itemController; 

    // Referencia a props colocados
    public GameObject[] ItemPrefabs;

    // Id de objeto seleccionado
    public int id;

    // Aparecer Prop
    public void SpawnProp(){
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        if(itemController.tempObject != null){
            EditModeSpawnerServerRpc(worldPosition.x, worldPosition.y, id);
        }
    }

    // Llamada al server para spawnear el objeto como objeto de network o como duplicado local
    [ServerRpc (RequireOwnership=false)] 
    void EditModeSpawnerServerRpc(float x, float y, int index){
        GameObject spawnedObject;
        spawnedObject = Instantiate(ItemPrefabs[index], new Vector3(x, y, 0), Quaternion.identity);
        if (spawnedObject.tag == "Spawner") {
            spawnedObject.GetComponent<NetworkObject>().Spawn();
        } else {
            EditModeSpawnerClientRpc(x, y, index);
        }
        
    }

    // Llamada a los clientes para spawnear el objeto como duplicado local
    [ClientRpc]
    void EditModeSpawnerClientRpc(float x, float y, int index){
        if(!IsServer){
            Instantiate(ItemPrefabs[index], new Vector3(x, y, 0), Quaternion.identity);
        }
    }
}
