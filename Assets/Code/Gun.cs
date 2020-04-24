using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {
    public GameObject bulletPrefab;
    public Transform firePoint;

    public float bulletSpeed = 10f;
    public float fireRate = 1f;

    private float timeToFire = 0f;
    private bool found = false;

    // Start is called before the first frame update
    void Start() {
        if(bulletPrefab == null)
        {
            Debug.LogError("Gun: No bullet is attached!");
        }
        if(firePoint == null)
        {
            Debug.LogError("Gun: No Fire Point is found");
        }
    }

<<<<<<< Updated upstream
  // Update is called once per frame
    void Update() {
        HandleMouse();
=======
    // Update is called once per frame
    public override void UpdateEntity() {
        if (!found)
        {
            PlayerEntity player;
            if (UnitManager.Local.players.TryGetValue(authorityID, out player))
            {
                transform.parent = player.transform.Find("Hand");
                transform.localPosition = Vector3.zero;

                found = true;
            }
        }

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
>>>>>>> Stashed changes
    }

    void HandleMouse(){
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        transform.rotation = Quaternion.LookRotation(ray.direction, Vector3.up);

        if (Input.GetMouseButton(0) && Time.time >= timeToFire)
        {
            Shoot(ray.direction);
            timeToFire = Time.time + 1 / fireRate;
        }
    }

    void Shoot(Vector3 direction)
    {
<<<<<<< Updated upstream
        Transform bulletClone = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation).transform;
        //bulletClone.Translate(bulletSpeed * direction * Time.deltaTime);
=======
        var entity = MoveBullet.CreateEntity() as MoveBullet;

        entity.transform.position = firePoint.position;
        entity.transform.rotation = firePoint.rotation;
        entity.moveSpeed = bulletSpeed;
>>>>>>> Stashed changes

        UnitManager.Local.Register(entity);
    }
}
