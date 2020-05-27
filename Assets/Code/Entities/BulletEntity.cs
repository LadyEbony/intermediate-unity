using System.Collections;
using UnityEngine;

public enum DamageType : byte { Normal, Pure, Fire, Poison }

public class BulletEntity : EntityUnit {

  [Header("Network Data")]
  public Vector3 startingPosition;
  public Quaternion startingRotation;
  public float moveSpeed;
  public float timer;

  [Header("Network Data (Combat)")]
  public int baseDamage;
  public DamageType damageType;
  public int reflection;

  private float destroyTimer;

  // public float damageModifier = 0.1f; //percentage of bullet damage applied to the special damage type, ex. fire.

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
    if (destroyTimer <= Time.time){
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
    h.Add('t', timer);

    h.Add('d', (byte)baseDamage);
    h.Add('b', (byte)damageType);
    h.Add('e', (byte)reflection);
  }

  // Only called on others' clients
  public override void Deserialize(ExitGames.Client.Photon.Hashtable h) {
    base.Deserialize(h);

    object val;

    // NEW: While setting transform.position works in Deserialize()
    // NEW: Network packets are recieved in random timings
    // NEW: As such, you may have saw the bullets glitch a little
    // NEW: To remedy this, we only set the data at the start and hope nothing goes wrong

    // Accessing the data we previously stored in Serialize
    
    if (h.TryGetValue('s', out val)) {
      startingPosition = (Vector3)val;
    }

    if (h.TryGetValue('r', out val)) {
      startingRotation = (Quaternion)val;
    }

    if (h.TryGetValue('f', out val)) {
      moveSpeed = (float)val;
    }

    if (h.TryGetValue('t', out val)){
      timer = (float)val;
    }

    if (h.TryGetValue('d', out val)){
      baseDamage = (byte)val;
    }

    if (h.TryGetValue('b', out val)){
      damageType = (DamageType)(byte)val;
    }

    if (h.TryGetValue('e', out val)){
      reflection = (byte)val;
    }

  }

  // NEW: Rigidbodies have the ability to calculate contact points
  // NEW: This lets us easily do reflection
  // NEW: As a result, you need to take control of the rigidbody with isKinematic = true
  public void OnCollisionEnter(Collision collision) {
    // Pretend there is no latency
    // anyone can do the whole bounce

    // if (!isMine) return;

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

  // Hopingfully only Damage trigger will call this
  public void OnTriggerEnter(Collider collision){

    var entity = collision.gameObject.GetComponentInParent<CharacterEntity>();
    if (entity != null && entity.isMine){
      // bullets can only hurt things you own
      // meaning only yourself
      // and the host can only hurt ai
      
      // NEW: damage is calculated beforehand by the damage type
      // NEW: damage modifiers moved to gun
      UnitManager.Local.RaiseEvent('d', true, entity.entityID, (byte)baseDamage, (byte)damageType);


      UnitManager.Local.RaiseEvent('b', true, entityID, authorityID);
    }

  }

  /// <summary>
  /// Destroys bullet if you own it. Otherwise disables it.
  /// </summary>
  public void DestroyBullet() {
    if (isMine){
      // Deregister bullet so it can STOP appearing on other people's clients
      UnitManager.Local.Deregister(this);

      // Then we destroy it normally
      DestroyEntity();
      Destroy(gameObject);
    } else {
      DestroyEntity();
      gameObject.SetActive(false);
    }
  }
}
