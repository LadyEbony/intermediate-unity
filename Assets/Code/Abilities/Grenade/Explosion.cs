using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class Explosion : Projectile {

  [Header("Explosion")]
  public  float     explosionRadius;
  public  LayerMask explosionLayer;
  public GameObject explostionEffect;
  
  public override void Explode() {
    //destroy everything in the radius and destroy this object
    Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, explosionLayer);
    for (int i = 0; i < hitColliders.Length; ++i) {
        Destroy(hitColliders[i].gameObject);
    }

    // Deregister explosion so it can STOP appearing on other people's clients
    if (isMine){
      UnitManager.Local.Deregister(this);
      Destroy(gameObject);
    }

    // particles
    var eft = Instantiate(explostionEffect, transform.position, Quaternion.identity);
    Destroy(eft, 2f);
  }
}
