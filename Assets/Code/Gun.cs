using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {
    public GameObject bulletPrefab;
    public Transform firePoint;

    public float bulletSpeed = 10f;
    public float fireRate = 1f;
    public float bulletDestroyTime = 5f;

    private float timeToFire = 0f;

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

  // Update is called once per frame
    void Update() {
        HandleMouse();
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
        Transform bulletClone = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation).transform;
        bulletClone.Translate(bulletSpeed * direction * Time.deltaTime);

        Destroy(bulletClone.gameObject, bulletDestroyTime);
    }
}
