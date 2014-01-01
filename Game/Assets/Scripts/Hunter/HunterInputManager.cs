using UnityEngine;
using System.Collections;

public class HunterInputManager : MonoBehaviour {

	private HunterController hunterController;
	public HunterCamera hunterCamera;

	public HunterController HunterControllerReference {
		set {
			hunterController = (HunterController)value;
			hunterCamera = Camera.main.GetComponent<HunterCamera>();
			hunterCamera.SetTargets(hunterController.cameraTargetTransform,hunterController.hunterCameraTransform);
		}
	}
	

	void Update() {
		Vector2 moveDirection = new Vector2 (Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
		Vector2 aimDirection = new Vector2 (Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

		if(!hunterController) return;
		hunterController.GetUserInputs(moveDirection.x,moveDirection.y);
		if(Input.GetButtonDown("Fire1")){hunterController.StartFire();}
		if(Input.GetButtonUp("Fire1")){hunterController.StopFire();}
		if(Input.GetButtonDown("Jump")){hunterController.StartJump();}
		if(Input.GetButtonUp("Jump")){hunterController.StopJump();}

		if(!hunterCamera) return;
		hunterCamera.GetInput(aimDirection.x, aimDirection.y);

		RaycastHit hit;
		Ray hitt = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2,0));
		Physics.Raycast(hitt,out hit);
		hunterController.LookAt(hit.point);

		//TODO: Remove this debuging
		//Vector3 dir = hunterController.shotPosition.position.normalized;
		//Vector3 to = new Vector3(hunterController.shotPosition.position.x*dir.x,hunterController.shotPosition.position.y*dir.y,hunterController.shotPosition.position.z*dir.z)*5;
		Debug.DrawLine(hunterController.shotPosition.position,hit.point,Color.red);
	}
}
