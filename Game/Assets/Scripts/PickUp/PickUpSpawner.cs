using UnityEngine;
using System.Collections;

public class PickUpSpawner : Photon.MonoBehaviour {

	[SerializeField] private float spawnTime = 10;
	[SerializeField] private GameObject pickUp = null;
	private GameObject pickUpClone = null;
	
	public void OnJoinedRoom()	{
		if(PhotonNetwork.isMasterClient) {
			StartCoroutine(this.SpawnPickUp(0));
		}
	}
	
	IEnumerator SpawnPickUp(float _delay) {
		yield return new WaitForSeconds(_delay);
		pickUpClone = PhotonNetwork.Instantiate(pickUp.name,transform.position,transform.rotation,0);
		pickUpClone.transform.parent = this.transform;
		pickUpClone.GetComponent<PickUp>().pickUpSpawner = this;
	}

	public void RemovePickUp() {
		photonView.RPC("RemovePickUpRPC", PhotonTargets.All);
	}

	[RPC]
	private void RemovePickUpRPC() {
		if(pickUpClone && PhotonNetwork.isMasterClient) {
			PhotonNetwork.Destroy(pickUpClone);
			StartCoroutine(this.SpawnPickUp(spawnTime));
		}
	}
}
