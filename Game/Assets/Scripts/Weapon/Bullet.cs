using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet : MonoBehaviour {

	public Color particleColor = Color.white;
	public HunterController bulletOwner = null;

	// Update is called once per frame
	void Update () {
		transform.Translate(Vector3.forward * 50 * Time.deltaTime);
	}

	void OnTriggerEnter(Collider c) {
		if(bulletOwner) {
			if(c.gameObject.tag == "Player") {
				HunterController playerHit = c.gameObject.GetComponent<HunterController>();
				if(playerHit) {
					playerHit.TakeDamage(1);
				}
			}
		}

		if(c.gameObject.tag != "PickUp")
			Destroy(this.gameObject);
	}
}
