using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Weapons {
	WEAPON_FAKE_ONE,	// Assalut Rifle
	WEAPON_FAKE_TWO,	// Burst Assalut Rifle 
	WEAPON_FAKE_THREE,  // Pistol
}

public class HunterController : Photon.MonoBehaviour {

	// Hunter Movement serialize in the inspector,
	//can be public in the future, but should be private for now
	[SerializeField] private float runSpeed = 10f;
	[SerializeField] private float walkSpeed = 10f;
	[SerializeField] public float jumpHeight = 2f;
	[SerializeField] private float maxRotationSpeed = 1000f;
	[SerializeField] private Renderer[] bodyRenderers = null;
	[SerializeField] private int health = 3;

	private Dictionary<Weapons,WeaponManager> weapons = new Dictionary<Weapons, WeaponManager>();

	public Transform cameraTargetTransform;
	public Transform hunterCameraTransform;
	public Transform headPivot;
	public float idleTimer;
	public Weapons currentWeapon = Weapons.WEAPON_FAKE_ONE;
	
	// Public variables hidden in inspector
	[HideInInspector] public bool walk;
	[HideInInspector] public bool crouch;
	[HideInInspector] public bool inAir;
	[HideInInspector] public bool fire;
	[HideInInspector] public bool aim;
	[HideInInspector] public bool reloading;
	[HideInInspector] public string currentWeaponName;
	[HideInInspector] public bool grounded;
	[HideInInspector] public float targetYRotation;
	[HideInInspector] public bool dead = false;
	[HideInInspector] public Vector3 moveDir;
	[HideInInspector] public Transform shotPosition = null;
	
	// Private variables	
	private Transform hunterTransform;
	private CharacterMotor motor;	
	private Color myColor = Color.white;
	private int connectedPlayers = 0;
	private Vector3 startingPosition;
	private bool firing;
	private float firingTimer;
	private int startingHealth;

	public int Health {
		get{return health;}
		set{health = value;}
	}
	

	void Start() {
		idleTimer = 0.0f;		
		hunterTransform = transform;
		
		this.walk = true;
		this.aim = false;
		this.reloading = false;
		this.startingPosition = this.transform.position;
		this.startingHealth = this.Health;

		motor = this.gameObject.GetComponent<CharacterMotor>();
		motor.CanControl = true;

		
		GameController.players.Add(this);
		connectedPlayers = GameController.players.Count;

		if(photonView.isMine) {
			myColor = new Color(Random.value,Random.value,Random.value);
			photonView.RPC("UpdateColor",PhotonTargets.All,myColor.r,myColor.g,myColor.b);
		}

		//add all the weapons to the weapons list
		weapons.Add(Weapons.WEAPON_FAKE_ONE,   new WeaponFakeOne(this));
		weapons.Add(Weapons.WEAPON_FAKE_TWO,   new WeaponFakeTwo(this));
		weapons.Add(Weapons.WEAPON_FAKE_THREE, new WeaponFakeThree(this));
	}
	
	void OnEnable()	{
		moveDir = Vector3.zero;
		walk = true;
		aim = false;
	}
	
	void OnDisable() {
		moveDir = Vector3.zero;
		walk = true;
		aim = false;
	}
	
	void Update() {

		if(Input.GetKeyDown(KeyCode.K)){
			Death ();
		}

		//check to see if there is a new comer
		if(connectedPlayers != GameController.players.Count) {
			connectedPlayers = GameController.players.Count;
			photonView.RPC("UpdateColor",PhotonTargets.All,myColor.r,myColor.g,myColor.b);
		}

		if(this.motor.CanControl) {
			idleTimer += Time.deltaTime;
					
			//Check the hunter move direction
			if (moveDir.sqrMagnitude > 1) {
				moveDir = moveDir.normalized;
			}

			motor.InputMoveDirection = transform.TransformDirection(moveDir);

			motor.movement.maxForwardSpeed = ((walk) ? ((crouch) ? 0 : walkSpeed) : ((crouch) ? 0 : runSpeed));//don't move if it is crouch
			motor.movement.maxBackwardsSpeed = motor.movement.maxForwardSpeed;
			motor.movement.maxSidewaysSpeed = motor.movement.maxForwardSpeed;


			if(moveDir != Vector3.zero) {
				idleTimer = 0.0f;
			}

			inAir = !motor.grounded;

			float currentAngle = hunterTransform.localRotation.eulerAngles.y;
			float delta = Mathf.Repeat ((targetYRotation - currentAngle), 360);
			if (delta > 180) {
				delta -= 360;
			}

			hunterTransform.localEulerAngles = new Vector3(hunterTransform.localRotation.eulerAngles.x,Mathf.MoveTowards(currentAngle, currentAngle + delta, Time.deltaTime * maxRotationSpeed),hunterTransform.localRotation.eulerAngles.z);
		}

		weapons[currentWeapon].OnFire();
	}
	
	public void GetUserInputs(float _hori, float _vert) {

		if(!dead) {
			moveDir = new Vector3(_hori, 0, _vert);
		} else {
			moveDir = Vector3.zero;
			if(motor)motor.CanControl = false;
		}
	}

	public void StartJump() {StartJump(jumpHeight);}
	public void StartJump(float _height) {
		if(motor) {
			motor.jumping.baseHeight = _height;
			motor.jumping.extraHeight = _height + 0.5f;
			motor.InputJump = true;
		}
	}

	public void StopJump() {
		motor.InputJump = false;
	}

	public IEnumerator AutoStopJump() {
		yield return null;
		motor.InputJump = false;
	}

	public void LookAt (Vector3 point) {
		this.headPivot.LookAt(point);
	}
	
	public void StartFire() {
		weapons[currentWeapon].StartFire();
	}
	
	
	public void StopFire() {
		weapons[currentWeapon].StopFire();
	}

	public void FireBullet(string _bulletName) {
		photonView.RPC("FireBulletRPC",PhotonTargets.All, shotPosition.position,shotPosition.rotation,_bulletName);
	}

	public void TakeDamage(int _damage) {
		photonView.RPC("TakeDamageRPC",PhotonTargets.All, _damage);
	}

	public void Death() {
		//TODO:Fix network dying bug
		/*this.transform.position = startingPosition;
		this.transform.rotation = Quaternion.identity;
		this.health = this.startingHealth;
		NetworkCharacter nc = this.GetComponent<NetworkCharacter>();
		if(nc) {
			nc.IgnoreLerp();
		}*/
	}
	
	[RPC]
	void FireBulletRPC(Vector3 _v, Quaternion _q, string _bulletName ,PhotonMessageInfo _info) {
		GameObject bullet = Resources.Load(_bulletName) as GameObject;
		if(bullet) {
			Instantiate(bullet,_v,_q);
			Bullet bScript = bullet.GetComponent<Bullet>();
			if(photonView.ownerId == PhotonNetwork.player.ID) {
				bScript.bulletOwner = this;
			}
		}
	}
	
	[RPC]
	void UpdateColor(float r, float g, float b) {
		
		myColor = new Color(r,g,b);
		
		foreach(Renderer ren in bodyRenderers) {
			ren.material.color = myColor;
		}
	}

	[RPC]
	void TakeDamageRPC(int _damage) {
		this.Health -= _damage;
		if(this.Health <= 0){
			Death ();
		}
	}

	//TODO: This is for debugging only
	public void Print(string msg) {
		print (msg);
	}
}
