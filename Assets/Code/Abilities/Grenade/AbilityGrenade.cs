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
        var parent = GetComponentInParent<PlayerEntity>().gun.firePoint;

        newGrenade.transform.position = parent.position;
        newGrenade.rb.velocity = parent.rotation * Vector3.forward * speed;

        // Register explosion so it can appear on other people's clients
        UnitManager.Local.Register(newGrenade);
    }

}
