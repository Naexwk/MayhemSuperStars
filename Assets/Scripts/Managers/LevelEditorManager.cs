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
    public GameObject bombExplosion;

    // Id de objeto seleccionado
    public int id;

    // Aparecer Prop
    public void SpawnProp(){
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        if(itemController.tempObject != null){
            if(itemController.tempObject.tag != "Bomb"){
                EditModeSpawnerServerRpc(worldPosition.x, worldPosition.y, id);
            } else{
                BombServerRpc(worldPosition.x, worldPosition.y);
            }
        }
    }
    [ServerRpc (RequireOwnership=false)] 
    void BombServerRpc(float x, float y){
        Debug.Log("Bomb Server Rpc");
        GameObject spawnedObject;
        spawnedObject = Instantiate(bombExplosion, new Vector3(x, y, 0), Quaternion.identity);
        BombClientRpc(x, y);
    }
    // Llamada al server para spawnear el objeto como objeto de network o como duplicado local
    [ServerRpc (RequireOwnership=false)] 
    void EditModeSpawnerServerRpc(float x, float y, int index){
        GameObject spawnedObject;
        spawnedObject = Instantiate(ItemPrefabs[index], new Vector3(x, y, 0), Quaternion.identity);
        EditModeSpawnerClientRpc(x, y, index);
    }

    // Llamada a los clientes para spawnear el objeto como duplicado local
    [ClientRpc]
    void EditModeSpawnerClientRpc(float x, float y, int index){
        if(!IsServer){
            Instantiate(ItemPrefabs[index], new Vector3(x, y, 0), Quaternion.identity);
        }
    }
    [ClientRpc]
    void BombClientRpc(float x, float y){
        if(!IsServer){
            Instantiate(bombExplosion, new Vector3(x, y, 0), Quaternion.identity);
        }
    }
}
