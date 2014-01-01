using UnityEngine;
using System.Collections;

public class UICamera : MonoBehaviour {

	[SerializeField] private bool constantAnchorUpdate = false;

	[SerializeField] private Transform TopLeft = null;
	[SerializeField] private Transform TopCenter = null;
	[SerializeField] private Transform TopRight = null;
	[SerializeField] private Transform MiddleLeft = null;
	[SerializeField] private Transform MiddleCenter = null;
	[SerializeField] private Transform MiddleRight = null;
	[SerializeField] private Transform BottomLeft = null;
	[SerializeField] private Transform BottomCenter = null;
	[SerializeField] private Transform BottomRight = null;

	void Start () {
		UpdateAnchors();
	}
	
	void Update() {
		if(constantAnchorUpdate)UpdateAnchors();
	}

	void UpdateAnchors() {

		var w = Screen.width;
		var h = Screen.height;

		TopLeft.position = GetAnchorWorldPosition		(new Vector2(0		,h		));
		TopCenter.position = GetAnchorWorldPosition		(new Vector2(w*0.5f	,h		));
		TopRight.position = GetAnchorWorldPosition		(new Vector2(w		,h		));
		MiddleLeft.position = GetAnchorWorldPosition	(new Vector2(0		,h*0.5f	));
		MiddleCenter.position = GetAnchorWorldPosition	(new Vector2(w*0.5f	,h*0.5f	));
		MiddleRight.position = GetAnchorWorldPosition	(new Vector2(w		,h*0.5f	));
		BottomLeft.position = GetAnchorWorldPosition	(new Vector2(0		,0		));
		BottomCenter.position = GetAnchorWorldPosition	(new Vector2(w*0.5f	,0		));
		BottomRight.position = GetAnchorWorldPosition	(new Vector2(w		,0		));
	}

	Vector3 GetAnchorWorldPosition(Vector2 cordinate) {
		Ray ray = this.camera.ScreenPointToRay(new Vector3(cordinate.x,cordinate.y,0));
		Vector3 v = new Vector3(ray.origin.x,ray.origin.y,5);
		return v;
	}
}
