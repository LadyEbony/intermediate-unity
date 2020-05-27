using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class Explosion : Projectile {

  [Header("Explosion")]
  public  float     explosionRadius;
  public  LayerMask explosionLayer;
  public GameObject explostionEffect;
  private bool exploded;

  public new static EntityUnit CreateEntity(){
    return SetEntityHelper(GameInitializer.Instance.grenadePrefab);
  }

  public override void Explode() {

    if (exploded){
      return;
    }

    if (isMine){
      //destroy everything in the radius and destroy this object
      var hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, explosionLayer, QueryTriggerInteraction.Collide);
      foreach(var c in hitColliders){
        var entity = c.transform.GetComponentInParent<CharacterEntity>();
        if (entity){
          UnitManager.Local.RaiseEvent('d', true, entity.entityID, (byte)100, (byte)DamageType.Fire);
        }
      }

      UnitManager.Local.Deregister(this);
      Destroy(gameObject);
    }

    // particles
    var eft = Instantiate(explostionEffect, transform.position, Quaternion.identity);
    Destroy(eft, 2f);

    exploded = true;
  }

  private void OnDrawGizmos() {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, explosionRadius);
  }

}
