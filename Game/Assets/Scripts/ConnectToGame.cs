using UnityEngine;
using System.Collections;

public class ConnectToGame : MonoBehaviour {
	
	public bool AutoConnect = true;
	private bool ConnectInUpdate = true;
	
	public virtual void Start() {
		PhotonNetwork.autoJoinLobby = false;
	}
	
	public virtual void Update() {
		if (ConnectInUpdate && AutoConnect) {
			Debug.Log("Update() was called by Unity. Scene is loaded. Let's connect to the Photon Master Server. Calling: PhotonNetwork.ConnectUsingSettings();");
			
			ConnectInUpdate = false;
			PhotonNetwork.ConnectUsingSettings("v0.0.01");
		}
	}

	public virtual void OnConnectedToMaster()	{
		Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
		PhotonNetwork.JoinRandomRoom();
	}
	
	public virtual void OnPhotonRandomJoinFailed()	{
		Debug.Log("OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, true, true, 4);");
		PhotonNetwork.CreateRoom(Mathf.Round(UnityEngine.Random.value*100).ToString(), true, true, 20);
	}
	
	public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)	{
		Debug.LogError("Cause: " + cause);
	}
	
	public void OnJoinedRoom()	{
		Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");

		GameObject myCharacter  = PhotonNetwork.Instantiate("Hunter",new Vector3(0,5.5f,0),Quaternion.identity,0);
		this.GetComponent<HunterInputManager>().HunterControllerReference = myCharacter.GetComponent<HunterController>();
	}
	
	public virtual void OnJoinedLobby()	{
		Debug.Log("OnJoinedLobby(). Use a GUI to show existing rooms available in PhotonNetwork.GetRoomList().");
	}
}
