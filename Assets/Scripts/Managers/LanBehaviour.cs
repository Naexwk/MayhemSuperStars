using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using UnityEngine.SceneManagement;

// Controlador de sesión de red
public class LanBehaviour : NetworkBehaviour
{
	// Referencia a protocolo de transporte de Unity, dentro del NetworkManager
	[SerializeField] UnityTransport transport;
	
	// Prefab de GameManager
	[SerializeField] GameObject gameManagerPrefab;
	[SerializeField] GameObject localGameManagerPrefab;

	// Código de sala de juego (cliente)
	private string inputJoinCode;
	// Código de sala de juego (host)
	public string hostJoinCode;

	// Nombre de este jugador
	public string playerName = "Anon";
	// Referencia a campo de texto de nombre
	[SerializeField] GameObject tf_newName;
	// Referencia a campo de texto para recibir código de sala de juego
	[SerializeField] TMP_InputField joinCodeText;

	[SerializeField] private GameObject loadingPanel;

	void Awake () {
		DontDestroyOnLoad(this.gameObject);
	}

	// Inicializar Autenticación anónima
	async void Start()
	{
		await UnityServices.InitializeAsync();
		await AuthenticationService.Instance.SignInAnonymouslyAsync();
	}

	// Inicia el juego como Host
	public void StartHost() {
		CreateRelay();
	}

	// Hostea un juego y añade un GameManager
	public async void CreateRelay(){
		try {
			loadingPanel.SetActive(true);
			// Crear sesión de juego y obtener código de sala de juego
			Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
			string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
			hostJoinCode = joinCode;
			RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
			NetworkManager.Singleton.StartHost();
			
			// Crear un GameManager
			InstantiateGameManager();

			// Cambiar de escena
			ChangeScene("GameRoom");
		} catch (RelayServiceException e){
			loadingPanel.SetActive(false);
			Debug.Log(e);
		}
	}

	// Inicia el juego como cliente
	public void StartClient() {
		// Entrar a sesión de juego con el código de sala en el campo de texto
		inputJoinCode = joinCodeText.text;
		JoinRelay(inputJoinCode);
	}

	public async void JoinRelay(string joinCode){
		try {
			loadingPanel.SetActive(true);
			// Entrar a sesión de juego
			hostJoinCode = joinCode.ToUpper();
			JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
			RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
			NetworkManager.Singleton.StartClient();
		} catch (RelayServiceException e){
			loadingPanel.SetActive(false);
			Debug.Log(e);
		}
	}

	public void StartLocalGame(){
		try {
			//Debug.Log(NetworkManager.Singleton.GetComponent<UnityTransport>().ProtocolType);
			NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("127.0.0.1", (ushort)7777);
			loadingPanel.SetActive(true);
			// Crear sesión de juego y obtener código de sala de juego
			NetworkManager.Singleton.StartHost();
			
			// Crear un GameManager
			InstantiateGameManager();

			// Cambiar de escena;
			ChangeScene("LocalGameRoom");
		} catch (RelayServiceException e){
			loadingPanel.SetActive(false);
			Debug.Log(e);
		}
	}

	// Instancia un GameManager Host-Only
	public void InstantiateGameManager(){
		GameObject gameManager;
		gameManager = Instantiate(gameManagerPrefab, transform.position,Quaternion.identity);
		gameManager.GetComponent<NetworkObject>().Spawn();
	}

	// Instancia un GameManager Local-Only
	public void InstantiateLocalGameManager(){
		GameObject gameManager;
		gameManager = Instantiate(localGameManagerPrefab, transform.position,Quaternion.identity);
		gameManager.GetComponent<NetworkObject>().Spawn();
	}

	// Función de cambio de escena
	public void ChangeScene(string sceneName){
		NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
	}

	// Función para cambiar de nombre
	public void changeName(){
		playerName = tf_newName.GetComponent<TMP_InputField>().text;
	}


}