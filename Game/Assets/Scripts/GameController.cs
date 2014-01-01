using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public static List<HunterController> players = new List<HunterController>();

	void Start() {
		Screen.lockCursor = true;
	}

	void Update () {
		if(Input.GetButtonDown("Fire2")) {
			Screen.showCursor = !Screen.showCursor;
			Screen.lockCursor = !Screen.lockCursor;
		}
	}
}
