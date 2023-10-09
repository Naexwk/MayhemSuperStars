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

public class LanBehaviour : NetworkBehaviour
{
	private PlayerController pc;
	private bool pcAssigned;

	//[SerializeField] TextMeshProUGUI ipAddressText;
	[SerializeField] TMP_InputField joinCodeText;

	//[SerializeField] string ipAddress;
	[SerializeField] UnityTransport transport;

	public GameObject gameManagerObj;
	//public GameObject playButton;
	string inputJoinCode;
	public string hostJoinCode;
	public string playerName = "Anon";

	public GameObject  tf_newName;

	void Awake () {
		DontDestroyOnLoad(this.gameObject);
	}

	async void Start()
	{
		//ipAddress = "0.0.0.0";
		//SetIpAddress();
		await UnityServices.InitializeAsync();
		await AuthenticationService.Instance.SignInAnonymouslyAsync();
		//await AuthenticationService.Instance.UpdatePlayerNameAsync("Anon");
		//Debug.Log(await AuthenticationService.Instance.GetPlayerNameAsync());

	}

	// Hostea un juego y añade un GameManager
	public void StartHost() {
		//Debug.Log(HostGame(3));
		CreateRelay();
		
		//GetLocalIPAddress(); 
		

	}

	// Inicia un cliente
	public void StartClient() {
		// Obtiene la IP local y se une al juego con esa IP
		// Esto debe cambiar a conseguir la IP del campo de texto
		//ipAddress = GetLocalIPAddress(); 
		//ipAddress = ip.text;
		//SetIpAddress();
		inputJoinCode = joinCodeText.text;
		JoinRelay(inputJoinCode);
		
		
	}

	/* Gets the Ip Address of your connected network and
	shows on the screen in order to let other players join
	by inputing that Ip in the input field */
	// ONLY FOR HOST SIDE 
	/*public string GetLocalIPAddress() {
		var host = Dns.GetHostEntry(Dns.GetHostName());
		foreach (var ip in host.AddressList) {
			if (ip.AddressFamily == AddressFamily.InterNetwork) {
				ipAddress = ip.ToString();
				return ip.ToString();
			}
		}
		throw new System.Exception("No network adapters with an IPv4 address in the system!");
	}*/

	/* Sets the Ip Address of the Connection Data in Unity Transport
	to the Ip Address which was input in the Input Field */
	// ONLY FOR CLIENT SIDE
	/*public void SetIpAddress() {
		transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
		transport.ConnectionData.Address = ipAddress;
	}*/

	// Instancia un GameManager. Server-only
	public void InstantiateGameManager(){
		GameObject gameManagerPrefab;
		gameManagerPrefab = Instantiate(gameManagerObj, transform.position,Quaternion.identity);
		gameManagerPrefab.GetComponent<NetworkObject>().Spawn();
	}

	public void ChangeScene(string sceneName){
		NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
	}

	/*public void activatePlayButton(bool _state)
    {
        playButton.SetActive(_state);
    }*/

	public async void CreateRelay(){
		try {
			Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
			string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
			Debug.Log(joinCode);
			hostJoinCode = joinCode;

			RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
			NetworkManager.Singleton.StartHost();
			//ipAddressText.text = "Join Code: " + joinCode;
			InstantiateGameManager();
			ChangeScene("GameRoom");

		} catch (RelayServiceException e){
			Debug.Log(e);
		}
		
	}

	public async void JoinRelay(string joinCode){
		try {
			Debug.Log("Joining Relay with " + joinCode);
			JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
			RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
			NetworkManager.Singleton.StartClient();
		} catch (RelayServiceException e){
			Debug.Log(e);
		}
	}

	public void changeName(){
		playerName = tf_newName.GetComponent<TMP_InputField>().text;
		// DEV: Later
		//await AuthenticationService.Instance.UpdatePlayerNameAsync(newName);
		Debug.Log(playerName);
		//Debug.Log(await AuthenticationService.Instance.GetPlayerNameAsync());
	}

}