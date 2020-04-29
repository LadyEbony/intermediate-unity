using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityGrenade : Ability
{
    public GameObject grenade;
    public float speed;
    
    public override void Activate() {
        // Create explosion
        // This looks overcomplicated. But this is exactly how UnitManager does it
        var newGrenade = Explosion.CreateEntity() as Explosion;
        
        // Insert data into explosion
        newGrenade.transform.position = transform.position;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        newGrenade.rb.velocity = new Vector3(ray.direction.x * speed, ray.direction.y * speed, ray.direction.z * speed);

        // Register explosion so it can appear on other people's clients
        UnitManager.Local.Register(newGrenade);
    }

}
