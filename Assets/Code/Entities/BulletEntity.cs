using System.Collections;
using UnityEngine;

public class BulletEntity : EntityUnit {

  [Header("Network Data")]
  public Vector3 startingPosition;
  public Quaternion startingRotation;
  public float moveSpeed;

  [Header("Master Data")]
  public float timer;
  public int reflection, penetration;
  private float destroyTimer;

  [HideInInspector] public Rigidbody rb;

  // Necessary function
  // Creates an empty bullet prefab to place YOUR or OTHER'S data in
  // Requires the exact same header
  public new static EntityUnit CreateEntity() {
    return SetEntityHelper(GameInitializer.Instance.bulletPrefab);
  }

  public override void AwakeEntity() {
    base.AwakeEntity();

    rb = GetComponent<Rigidbody>();
  }

  // Called once before UpdateEntity()
  public override void StartEntity() {
    base.StartEntity();

    transform.position = startingPosition;
    transform.rotation = startingRotation;
    rb.position = startingPosition;
    rb.rotation = startingRotation;

    // NEW: Destroy gameobject after Time.time > destroyTimer
    // NEW: Only done to keep the update code in the same place
    destroyTimer = Time.time + timer;
  }

  // Called once every frame
  public override void UpdateEntity() {
    if (isMine && destroyTimer <= Time.time){
      DestroyBullet();
    }
  }

  private void FixedUpdate() {
    rb.MovePosition(rb.position + (startingRotation * Vector3.forward * moveSpeed) * Time.fixedDeltaTime);
  }

  // Only called on the local client
  public override void Serialize(ExitGames.Client.Photon.Hashtable h) {
    base.Serialize(h);

    //Add transform.position, transform.rotation, and movespeed into network message with the keys 's' 'r' and 'f'
    h.Add('s', startingPosition);
    h.Add('r', startingRotation);
    h.Add('f', moveSpeed);
  }

  // Only called on others' clients
  public override void Deserialize(ExitGames.Client.Photon.Hashtable h) {
    base.Deserialize(h);

    object val;

    // NEW: While setting transform.position works in Deserialize()
    // NEW: Network packets are recieved in random timings
    // NEW: As such, you may have saw the bullets glitch a little
    // NEW: To remedy this, we only set the data when it's different from 

    // Accessing the data we previously stored in Serialize
    
    bool changed = false;
    if (h.TryGetValue('s', out val)) {
      var sp = (Vector3)val;
      if (sp != startingPosition){
        changed = true;
        startingPosition = sp;
      }
    }

    if (h.TryGetValue('r', out val)) {
      var sr = (Quaternion)val;
      if (sr != startingRotation){
        changed = true;
        startingRotation = sr;
      }
    }

    if (h.TryGetValue('f', out val)) {
      moveSpeed = (float)val;
    }


    // For reflection
    if (changed){
      transform.position = startingPosition;
      transform.rotation = startingRotation;
      rb.position = startingPosition;
      rb.rotation = startingRotation;
    }
  }

  // NEW: Rigidbodies have the ability to calculate contact points
  // NEW: This lets us easily do reflection
  // NEW: As a result, you need to take control of the rigidbody with isKinematic = true
  public void OnCollisionEnter(Collision collision) {
    // Only master can calculate bullets
    if (!isMine) return;

    if (reflection == 0){
      DestroyBullet();
      return;
    }

    var contact = collision.contacts;
    if (contact.Length > 0){
      var p = contact[0].point;
      var n = contact[0].normal;

      startingPosition = p;
      startingRotation = Quaternion.LookRotation(Vector3.Reflect(startingRotation * Vector3.forward, n));      

      rb.position = startingPosition;
      rb.rotation = startingRotation;
    }

    reflection -= 1;
  }

  void DestroyBullet() {
    // Deregister bullet so it can STOP appearing on other people's clients
    UnitManager.Local.Deregister(this);

    // Then we destroy it normally
    DestroyEntity();
    Destroy(gameObject);
  }
}
