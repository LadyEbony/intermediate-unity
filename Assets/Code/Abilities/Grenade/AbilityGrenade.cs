using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityGrenade : Ability
{
    public GameObject grenade;
    public float speed;
    
    public override void Activate() {
        var newGrenade = Explosion.CreateEntity() as Explosion;

        newGrenade.transform.position = transform.position;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        newGrenade.rb.velocity = new Vector3(ray.direction.x * speed, ray.direction.y * speed, ray.direction.z * speed);

        UnitManager.Local.Register(newGrenade);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1)) { // Omar - changed to right mouse button
            //Debug.Log("Button press detected");
            Activate();
        }
    }
}
