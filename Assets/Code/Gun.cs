using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {
    public Transform firePoint;

    public float bulletSpeed = 10f;
    public float fireRate = 1f;

    private float timeToFire = 0f;

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

      if (Input.GetMouseButton(0) && Time.time >= timeToFire)
      {
        Shoot();
        timeToFire = Time.time + 1 / fireRate;
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
        var entity = MoveBullet.CreateEntity() as MoveBullet;

        // Insert data into bullet
        entity.trans = firePoint;
        entity.moveSpeed = bulletSpeed;
        entity.direction = Vector3.forward;

        // Register bullet so it can appear on other people's clients
        UnitManager.Local.Register(entity);
    }
}
