using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gun : MonoBehaviour {
    public Transform firePoint;

    [Header("Bullet Stats")]
    public float bulletSpeed = 10f;
    public float bulletAliveTime = 1f;
    public int bulletPenetration = 0;
    public int bulletReflection = 0;

    [Header("Gun Stats")]
    public float fireRate = 1f;
    private float timeToFire = 0f;
    public float angleDeviation = 2f;
    public int damage = 10;
    public DamageType[] ammoType;
    [HideInInspector] public int ammoTypePointer;
    public int effectTimer;
    public DamageType currentAmmoType => ammoType[ammoTypePointer];

    // I'm using bytes cause adding floats never works well
    [Header("Gun Ammo Stats")]
    public byte pureAmmoModifer = 50;
    public byte fireAmmoModifer = 120;
    public byte poisonAmmoModifer = 50;

    public event System.Action<Gun> onBulletFired;
    public event System.Action<Gun> onGunReloaded;

    public int GetDamage{
      get {
        switch(currentAmmoType){
          case DamageType.Normal:
            return damage;
          case DamageType.Pure:
            return (int)(damage * (pureAmmoModifer / 100f));
          case DamageType.Fire:
            return (int)(damage * (fireAmmoModifer / 100f));
          case DamageType.Poison:
            return (int)(damage * (poisonAmmoModifer / 100f));
        }
        return damage;
      }
    }

    public byte GetAmmoModifer{
      get {
        switch(currentAmmoType){
          case DamageType.Normal:
            return 100;
          case DamageType.Pure:
            return pureAmmoModifer;
          case DamageType.Fire:
            return fireAmmoModifer;
          case DamageType.Poison:
            return poisonAmmoModifer;
        }
        return 100;
      }
    }

    [Header("Magazine Stats")]
    public int ammoCount = 30;
    public int maxAmmoCount = 30;
    public float reloadTime = 1f;
    private float baseReloadTime, nextReloadTime;

  private void Awake() {
    // moved to Awake because the array needs to be created
    // when the script is created
    // not on the first Update() call

    // lazy way (but automated) to get all enums
    ammoType = System.Enum.GetValues(typeof(DamageType)).Cast<DamageType>().ToArray();
    //  ammoType = new DamageType[3];
    //  ammoType[0] = DamageType.Normal;
    //  ammoType[1] = DamageType.Pure;
    //  ammoType[2] = DamageType.Fire;
    ammoTypePointer = 0;
  }

  void Start() {

      if (firePoint == null) {
        Debug.LogError("Gun: No Fire Point is found");
      }
    }

    // Previously, the gun was independent of player entity
    // No longer the case. The player entity handles the gun
    // As a result, gun does not have to be a networked entity (player entity does all that!)
    public void HandleGun(){
      //HandleMouse();

      if (Input.GetKeyDown(KeyCode.R)){
        ammoCount = 0;
        baseReloadTime = Time.time;
        nextReloadTime = baseReloadTime + reloadTime;
      }

      // NEW: Reload gun
      if (ammoCount == 0 && Time.time >= nextReloadTime){
        ammoCount = maxAmmoCount;
        onGunReloaded?.Invoke(this);
      }

      if (ammoCount > 0 && Input.GetMouseButton(0) && Time.time >= timeToFire) {
        Shoot();

        onBulletFired?.Invoke(this);

        timeToFire = Time.time + 1 / fireRate;
        ammoCount -= 1;

        // NEW: Reload timers
        // NEW: Not doing the ammo count decrement in shoot, that way we can do more crazy shit later
        if (ammoCount <= 0){
          baseReloadTime = Time.time;
          nextReloadTime = baseReloadTime + reloadTime;
        }
      }
    }

    public float GetDisplayRatio{
      get {
        if (ammoCount > 0) 
          return (float)ammoCount / maxAmmoCount;
        return (Time.time - baseReloadTime) / (nextReloadTime - baseReloadTime);
      }
    }

    public string GetDisplayText{
      get {
        return string.Format("{0}/{1}", ammoCount, maxAmmoCount);
      }
    }

    // This function could come in use in the future
    void HandleMouse(){
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        transform.rotation = Quaternion.LookRotation(ray.direction, Vector3.up);
    }

    public void Shoot()
    {
        // Create bullet
        // This looks overcomplicated. But this is exactly how UnitManager does it
        var entity = BulletEntity.CreateEntity() as BulletEntity;

        // Insert data into bullet
        entity.startingPosition = firePoint.position;
        entity.startingRotation = firePoint.rotation * Quaternion.Euler(GetAngleDeviation, GetAngleDeviation, GetAngleDeviation);
        entity.moveSpeed = bulletSpeed;
        entity.timer = bulletAliveTime;
        entity.reflection = bulletReflection;
        entity.baseDamage = GetDamage;
        entity.damageType = currentAmmoType;
        entity.effectTimer = effectTimer;

        // Register bullet so it can appear on other people's clients
        UnitManager.Local.Register(entity);
    }

    // NEW: Bullets aren't perfectly straight
    // NEW: Let's add a little deviation
    float GetAngleDeviation{
      get {
        return Random.Range(-angleDeviation, angleDeviation);
      }
    }
}
