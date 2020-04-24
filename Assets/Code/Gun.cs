using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : EntityUnit {
    public GameObject bulletPrefab;
    public Transform firePoint;

    public float bulletSpeed = 10f;
    public float fireRate = 1f;
    public float bulletDestroyTime = 5f;

    private float timeToFire = 0f;

    public new static EntityUnit CreateEntity(){
      return SetEntityHelper(GameInitializer.Instance.gunPrefab);
    }

    public override void AwakeEntity()
    {
        base.AwakeEntity();
    }

    // Start is called before the first frame update
    public override void StartEntity()
    {
      // moves gun to hand
      var playerOwner = UnitManager.Local.players[authorityID];
      transform.parent = playerOwner.transform.Find("Hand");
      transform.localPosition = Vector3.zero;

        if(bulletPrefab == null)
        {
            Debug.LogError("Gun: No bullet is attached!");
        }
        if(firePoint == null)
        {
            Debug.LogError("Gun: No Fire Point is found");
        }
    }

    // Update is called once per frame
    public override void UpdateEntity() {
        if (isMine){
          HandleMouse();

          if (Input.GetMouseButton(0) && Time.time >= timeToFire)
          {
              Shoot();
              timeToFire = Time.time + 1 / fireRate;
          }
        }
    }

    public override void Serialize(ExitGames.Client.Photon.Hashtable h)
    {
        base.Serialize(h);

        h.Add('r', transform.rotation);
    }

    public override void Deserialize(ExitGames.Client.Photon.Hashtable h)
    {
        base.Deserialize(h);

        object val;
        if(h.TryGetValue('r',out val))
        {
            transform.rotation = (Quaternion)val;
        }
    }

    void HandleMouse(){
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        transform.rotation = Quaternion.LookRotation(ray.direction, Vector3.up);
    }

    void Shoot()
    {
        Transform bulletClone = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation).transform;
        bulletClone.parent = transform;

        Destroy(bulletClone.gameObject, bulletDestroyTime);
    }
}
