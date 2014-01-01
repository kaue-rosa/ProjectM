using UnityEngine;
using System.Collections;

public class WeaponManager {

	//public int bullets = 20;
	public int bulletsInARow = 3;
	public float timeIntervalBetweenBullets = 0.2f;
	public string bulletPrefab = "Bullet";
	public bool hasScope = false;
	public HunterController hunterController;

	private bool firing = false;
	private float currentTimeIntervalBetweenBullets = 0;
	private int currentBulletsInARow = 0;

	public virtual void StartFire() {
		firing = true;
	}

	public virtual void OnFire() {

		currentTimeIntervalBetweenBullets += Time.deltaTime;

		if(!firing)return;

		if(currentTimeIntervalBetweenBullets >= timeIntervalBetweenBullets && currentBulletsInARow < bulletsInARow) {
			currentTimeIntervalBetweenBullets = 0;
			currentBulletsInARow++;
			hunterController.FireBullet(this.bulletPrefab);
		}
	}

	public virtual void StopFire() {
		firing = false;
		currentTimeIntervalBetweenBullets = 0;
		currentBulletsInARow = 0;
	}

	public void Reload() {
		//TODO: Reload? Maybe we need just an UI first...
	}
}

//TODO: Think about real weapons 
public class WeaponFakeOne : WeaponManager {
	
	public WeaponFakeOne(HunterController _hc) {
		this.hunterController = _hc;

		this.bulletsInARow = 1;
		this.timeIntervalBetweenBullets = 0f;
	}
}

public class WeaponFakeTwo : WeaponManager {
	public WeaponFakeTwo(HunterController _hc) {
		this.hunterController = _hc;

		this.bulletsInARow = 200;
		this.timeIntervalBetweenBullets = 0.2f;
	}
}

public class WeaponFakeThree : WeaponManager {
	public WeaponFakeThree(HunterController _hc) {
		this.hunterController = _hc;

		this.bulletsInARow = 3;
		this.timeIntervalBetweenBullets = 0.1f;
	}
}
