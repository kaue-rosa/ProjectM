using UnityEngine;
using System.Collections;

public class HunterCamera : MonoBehaviour {

	private Transform hunterTargetTransform;
	private Transform hunterTransform;
	
	public Vector2 speed = new Vector2(135.0f, 135.0f);
	public Vector2 aimSpeed = new Vector2(70.0f, 70.0f);
	public Vector2 maxSpeed = new Vector2(100.0f, 100.0f);
	
	public int yMinLimit = -90;
	public int yMaxLimit = 90;
	
	public int normalFOV = 60;
	public int zoomFOV = 30;
	
	public float lerpSpeed = 8.0f;
	
	//private float distance = 10.0f;
	
	private float x = 0.0f;
	public float y = 0.0f;
	
	private Transform camTransform;
	private Quaternion rotation;
	private Vector3 position;
	private float deltaTime;
	
	private HunterController hunterController;
	
	public bool orbit;
	
	public LayerMask hitLayer;
	
	private Vector3 cPos;
	
	public Vector3 normalDirection;
	public Vector3 aimDirection;
	public Vector3 crouchDirection;
	public Vector3 aimCrouchDirection;
	
	public float positionLerp;
	
	public float normalHeight;
	public float crouchHeight;
	public float normalAimHeight;
	public float crouchAimHeight;
	public float minHeight;
	public float maxHeight;
	
	public float normalDistance;
	public float crouchDistance;
	public float normalAimDistance;
	public float crouchAimDistance;
	public float minDistance;
	public float maxDistance;
	
	private float targetDistance;
	private Vector3 camDir;
	private float targetHeight;
	
	
	public float minShakeSpeed;
	public float maxShakeSpeed;
	
	public float minShake;
	public float maxShake = 2.0f;
	
	public int minShakeTimes;
	public int maxShakeTimes;
	
	public float maxShakeDistance;
	
	private bool shake;
	private float shakeSpeed = 2.0f;
	private float cShakePos;
	private int shakeTimes = 8;
	private float cShake;
	private float cShakeSpeed;
	private int cShakeTimes;
	
	//private DepthOfField _depthOfFieldEffect;
	
	public void SetTargets(Transform _target, Transform _hunter) {

		this.hunterTargetTransform = _target;
		this.hunterTransform = _hunter;

		cShakeTimes = 0;
		cShake = 0.0f;
		cShakeSpeed = shakeSpeed;
		
		//_depthOfFieldEffect = gameObject.GetComponent("DepthOfField") as DepthOfField;
		
		if(hunterTargetTransform == null || hunterTransform == null) { 
			Destroy(this);
			return;
		}
		
		hunterTargetTransform.parent = null;
		camTransform = transform;
		
		Vector3 angles = camTransform.eulerAngles;
		x = angles.y;
		y = angles.x;

		hunterController = hunterTransform.GetComponent<HunterController>();;
		targetDistance = normalDistance;
		cPos = hunterTransform.position + new Vector3(0, normalHeight, 0);
	}
	
	void GoToOrbitMode(bool state) {
		orbit = state;
		hunterController.idleTimer = 0.0f;
	}
	
	void Update() {

		if(!hunterController) {
			return;
		}

		if(orbit && (hunterController.moveDir.x != 0.0 || hunterController.moveDir.z != 0.0 || hunterController.aim || hunterController.fire)) {
			GoToOrbitMode(false);
		}
		
		if(!orbit && hunterController.idleTimer > 0.1) {
			GoToOrbitMode(true);
		}
	}
	
	void LateUpdate () {
		if(!hunterTargetTransform || !hunterTransform) {
			return;
		}

		deltaTime = Time.deltaTime;
		RotateHunter();
		CameraMovement();
	}
	
	void CameraMovement() {
		if(hunterController.aim) {
			//(camera.GetComponent(DepthOfField) as DepthOfField).enabled = true;
			camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, zoomFOV, deltaTime * lerpSpeed);
			
			if(hunterController.crouch) {
				camDir = (aimCrouchDirection.x * hunterTargetTransform.forward) + (aimCrouchDirection.z * hunterTargetTransform.right);
				targetHeight = crouchAimHeight;
				targetDistance = crouchAimDistance;
			} else {
				camDir = (aimDirection.x * hunterTargetTransform.forward) + (aimDirection.z * hunterTargetTransform.right);
				targetHeight = normalAimHeight;
				targetDistance = normalAimDistance;
			}
		} else {

			camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, normalFOV, deltaTime * lerpSpeed);

			if(hunterController.crouch) {
				camDir = (crouchDirection.x * hunterTargetTransform.forward) + (crouchDirection.z * hunterTargetTransform.right);
				targetHeight = crouchHeight;
				targetDistance = crouchDistance;
			} else	{
				camDir = (normalDirection.x * hunterTargetTransform.forward) + (normalDirection.z * hunterTargetTransform.right);
				targetHeight = normalHeight;
				targetDistance = normalDistance;
			}
		}
		
		camDir = camDir.normalized;		
		HandleCameraShake();
		cPos = hunterTransform.position + new Vector3(0, targetHeight, 0);		
		RaycastHit hit;

		if(Physics.Raycast(cPos, camDir, out hit, targetDistance + 0.2f, hitLayer)) {
			float t = hit.distance - 0.1f;
			t -= minDistance;
			t /= (targetDistance - minDistance);
			
			targetHeight = Mathf.Lerp(maxHeight, targetHeight, Mathf.Clamp(t, 0.0f, 1.0f));
			cPos = hunterTransform.position + new Vector3(0, targetHeight, 0); 
		}
		
		if(Physics.Raycast(cPos, camDir, out hit, targetDistance + 0.2f, hitLayer)) {
			targetDistance = hit.distance - 0.1f;
		}

		Vector3 lookPoint = cPos;
		lookPoint += (hunterTargetTransform.right * Vector3.Dot(camDir * targetDistance, hunterTargetTransform.right));
		
		camTransform.position = cPos + (camDir * targetDistance);
		camTransform.LookAt(lookPoint);
		
		hunterTargetTransform.position = cPos;
		hunterTargetTransform.rotation = Quaternion.Euler(y, x, 0);
	}
	
	void HandleCameraShake() {
		if(shake) {
			cShake += cShakeSpeed * deltaTime;
			
			if(Mathf.Abs(cShake) > cShakePos) {
				cShakeSpeed *= -1.0f;
				cShakeTimes++;
				
				if(cShakeTimes >= shakeTimes) {
					shake = false;
				}
				
				if(cShake > 0.0) {
					cShake = maxShake;
				} else {
					cShake = -maxShake;
				}
			}

			targetHeight += cShake;
		}
	}
	
	void StartShake(float distance)	{
		float proximity = distance / maxShakeDistance;
		
		if(proximity > 1.0) return;
		proximity = Mathf.Clamp(proximity, 0.0f, 1.0f);
		proximity = 1.0f - proximity;
		
		cShakeSpeed = Mathf.Lerp(minShakeSpeed, maxShakeSpeed, proximity);
		shakeTimes = (int)Mathf.Lerp(minShakeTimes, maxShakeTimes, proximity);
		cShakeTimes = 0;
		cShakePos = Mathf.Lerp(minShake, maxShake, proximity);
		
		shake = true;
	}
	
	public void GetInput(float mouseX, float mouseY)	{


		if(!hunterTargetTransform || !hunterTransform) {
			return;
		}

		Vector2 a = hunterController.aim ? aimSpeed : speed;
		x += Mathf.Clamp(mouseX * a.x, -maxSpeed.x, maxSpeed.x) * deltaTime;
		y -= Mathf.Clamp(mouseY * a.y, -maxSpeed.y, maxSpeed.y) * deltaTime;
		y = ClampAngle(y, yMinLimit, yMaxLimit);
	}
	
	void RotateHunter() {
		if(!orbit) hunterController.targetYRotation = x;
	}
	
	static float ClampAngle(float angle, float min, float max) {
		if (angle < -360) {
			angle += 360;
		}
		
		if (angle > 360) {
			angle -= 360;
		}
		
		return Mathf.Clamp (angle, min, max);
	}
}
