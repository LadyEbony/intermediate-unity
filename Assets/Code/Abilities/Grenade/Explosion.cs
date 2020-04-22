using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class Explosion : EntityUnit {

  public Rigidbody rb;

  public  float     detonationTime;
  public  float     explosionRadius;
  public  LayerMask explosionLayer;
  public GameObject explostionEffect;

  private float timeElapsed;
    
  public new static EntityUnit CreateEntity(){
    return SetEntityHelper(GameInitializer.Instance.grenadePrefab);
  }

  public override void AwakeEntity() {
    base.AwakeEntity();

    rb = GetComponent<Rigidbody>();
  }

  public override void StartEntity() {
    base.StartEntity();

    timeElapsed = 0;
  }

  public override void UpdateEntity() {
    base.UpdateEntity();

    timeElapsed += Time.deltaTime;
    if (timeElapsed >= detonationTime) {
      Explode();
    }
  }

  // local client
  public override void Serialize(ExitGames.Client.Photon.Hashtable h) {
    base.Serialize(h);

    h.Add('p', transform.position);
    h.Add('v', rb.velocity);
  }

  // all other clients
  public override void Deserialize(ExitGames.Client.Photon.Hashtable h) {
    base.Deserialize(h);

    object val;
    if (h.TryGetValue('p', out val)){
      transform.position = (Vector3)val;
    }

    if (h.TryGetValue('v', out val)){
      rb.velocity = (Vector3)val;
    }
  }
  
  public void Explode() {
    //destroy everything in the radius and destroy this object
    Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, explosionLayer);
    for (int i = 0; i < hitColliders.Length; ++i) {
        Destroy(hitColliders[i].gameObject);
    }
      
    UnitManager.Local.Deregister(this);
    if (isMine)
      Destroy(gameObject);

    // particles
    var eft = Instantiate(explostionEffect, transform.position, Quaternion.identity);
    Destroy(eft, 2f);
  }
}
