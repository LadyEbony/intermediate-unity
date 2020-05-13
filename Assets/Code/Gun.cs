using System.Collections;
using System.Collections.Generic;
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

    [Header("Magazine Stats")]
    public int ammoCount = 30;
    public int maxAmmoCount = 30;
    public float reloadTime = 1f;
    private float baseReloadTime, nextReloadTime;

    void Start() {
      if(firePoint == null) {
        Debug.LogError("Gun: No Fire Point is found");
      }
    }

    // Previously, the gun was independent of player entity
    // No longer the case. The player entity handles the gun
    // As a result, gun does not have to be a networked entity (player entity does all that!)
    public void HandleGun(){
      //HandleMouse();

      // NEW: Reload gun
      if (ammoCount == 0 && Time.time >= nextReloadTime){
        ammoCount = maxAmmoCount;
      }

      if (ammoCount > 0 && Input.GetMouseButton(0) && Time.time >= timeToFire) {
        Shoot();
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

    void Shoot()
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
