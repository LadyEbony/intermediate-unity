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

  // Necessary function
  // Creates an empty explosion prefab to place YOUR or OTHER'S data in
  // Requires the exact same header
  public new static EntityUnit CreateEntity(){
    return SetEntityHelper(GameInitializer.Instance.grenadePrefab);
  }

  // Called once before StartEntity() and Deserialize()
  public override void AwakeEntity() {
    base.AwakeEntity();

    rb = GetComponent<Rigidbody>();
  }

  // Called once before UpdateEntity()
  public override void StartEntity() {
    base.StartEntity();

    timeElapsed = 0;
  }

  // Called once every frame
  public override void UpdateEntity() {
    base.UpdateEntity();

    timeElapsed += Time.deltaTime;
    if (timeElapsed >= detonationTime) {
      Explode();
    }
  }

  // Only called on the local client
  public override void Serialize(ExitGames.Client.Photon.Hashtable h) {
    base.Serialize(h);

    // Add transform.position and rb.velocity into network message with the keys 's' and 'v'
    h.Add('p', transform.position);
    h.Add('v', rb.velocity);
  }

  // Only called on others' clients
  public override void Deserialize(ExitGames.Client.Photon.Hashtable h) {
    base.Deserialize(h);

    object val;

    // Accessing the data we previously stored in Serialize
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
