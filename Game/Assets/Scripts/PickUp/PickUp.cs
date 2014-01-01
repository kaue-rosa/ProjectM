using UnityEngine;
using System.Collections;

public class PickUp : Photon.MonoBehaviour {

	[SerializeField] private Weapons pickUpWeapon = Weapons.WEAPON_FAKE_ONE;
	[SerializeField] private GameObject effect = null;

	[HideInInspector] public PickUpSpawner pickUpSpawner = null;

	void OnTriggerEnter(Collider c) {
		if(c.tag == "Player") {
			HunterController hc = c.gameObject.GetComponent<HunterController>();
			if(hc.photonView.isMine) {
				hc.currentWeapon = this.pickUpWeapon;
				effect.renderer.enabled = false;//fake the deactivation delay with just hiding the renderer
				photonView.RPC ("RemoveMeRPC",PhotonTargets.All);
			}
		}
	}

	[RPC]
	void RemoveMeRPC() {
		if(pickUpSpawner)
			pickUpSpawner.RemovePickUp();
	}
}
