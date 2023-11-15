
using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.Collections;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

public abstract class PlayerController : NetworkBehaviour
{
    [NonSerialized] public float aiPriority;
    [NonSerialized]public int maxHealth;
    [NonSerialized]public float currentHealth;
    [NonSerialized]public float playerSpeed;
    [NonSerialized]public int bulletDamage;
    [NonSerialized]public ulong playerNumber;
    [NonSerialized]public int fireRate; // en disparos por segundo
    [NonSerialized]public bool enableControl;
    [NonSerialized]public bool isInvulnerable;
    [NonSerialized]public float timeSinceLastAbility;
    [NonSerialized]public float abilityCooldown; // en segundos
    [NonSerialized]public Animator animator;
    [NonSerialized]public NetworkVariable<FixedString64Bytes> characterCode = new NetworkVariable<FixedString64Bytes>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public abstract void GetHit();
    public abstract void SpawnFakeBullet(float _bulletSpeed, Vector2 _direction, ulong _playerNumber, float _x, float _y);
    public abstract Task ChangeCharacter(string _characterCode);
    public abstract void Respawn();
    [ClientRpc]
    public virtual void StartCameraClientRpc(){
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
    public abstract void Despawn();
}
