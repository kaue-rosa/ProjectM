using UnityEngine;
using System.Collections;

public class JumpPad : MonoBehaviour {

	[SerializeField] private float jumpHeight = 5;
	private HunterController hc = null;

	void OnTriggerEnter(Collider c) {
		if(c.tag == "Player") {
			hc = c.gameObject.GetComponent<HunterController>();
			if(hc.photonView.isMine) {
				hc.StartJump(jumpHeight);
				StartCoroutine(hc.AutoStopJump());
			}
		}
	}
}
