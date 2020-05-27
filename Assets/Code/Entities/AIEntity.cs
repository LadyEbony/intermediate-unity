using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.AI;

public class AIEntity : CharacterEntity {

  private NavMeshAgent ai;

  [Header("AI Targeting")]
  public PlayerEntity target;
  public LayerMask targetLayerMask;
  public float searchTimer = 0.5f;
  private float nextSearchTimer;

  public float targetingSpeedModifier = 0.125f;

  [Header("Bullet")]
  public Transform barrelTransform;
  public int bulletDamage = 1;
  public float bulletFirerate = 10f;
  public float bulletSpeed = 10f;
  public float bulletAliveTime = 1f;
  private float timeToFire;

  // Necessary function
  // Creates an empty ai prefab to place YOUR or OTHER'S data in
  // Requires the exact same header
  public new static EntityUnit CreateEntity() {
      return SetEntityHelper(GameInitializer.Instance.aiPrefab);
  }

  public override void AwakeEntity() {
    base.AwakeEntity();

    ai = GetComponent<NavMeshAgent>();
  }

  public override void StartEntity() {
    base.StartEntity();

    ai.Warp(nextPosition);

    if (isMine){
      
    } else {
      // just like how the rigidbody is disabled for other clients
      // same applies here
      Destroy(ai);
    }
  }

  public override void UpdateEntity() {
    base.UpdateEntity();

    // If ai targeting something I own (me), and within sight, shoot
    if (target && target.isMine && IsPlayerRaycast() && Time.time >= timeToFire){
      var bullet = BulletEntity.CreateEntity() as BulletEntity;

      bullet.startingPosition = barrelTransform.position;
      bullet.startingRotation = Quaternion.LookRotation((target.head.position - barrelTransform.position).normalized);
      bullet.baseDamage = bulletDamage;
      bullet.moveSpeed = bulletSpeed;
      bullet.timer = bulletAliveTime;
      bullet.reflection = 0;

      UnitManager.Local.Register(bullet);

      timeToFire = Time.time + 1f / bulletFirerate;
    }

    if (health <= 0)
      DestroyAI();
  }

  protected override void LocalUpdate() {
    base.LocalUpdate();

    // if the target ever disappears
    // or the search timer is triggered
    if (target == null || target.Equals(null) || Time.time >= nextSearchTimer){
      // find new target
      target = UnitManager.Local.GetClosestPlayerEntity(transform.position);
      nextSearchTimer = Time.time + (searchTimer * (1f + Random.value));  // adding some variance so we don't search on the same timings
    }

    
    if (target){
      // walk to player
      ai.destination = target.transform.position;

      // slow down if player in sight
      var speed = GetCurrentSpeed;
      if (IsPlayerRaycast()) speed *= targetingSpeedModifier;
      ai.speed = speed;
    }

  }

  protected override void RemoteUpdate() {
    base.RemoteUpdate();

    // t will require a value starting from 0, moving towards 1
    // since the next network message CAN return after updateTimer
    // t could return a value greater than 1
    // as a result, we multiply updateTimer by 1.5 so t is less likely to pass 1 (it's like a grace period)
    var t = (Time.time - baseUpdateTime) / (updateTimer * 1.5f);

    // Lerp returns a value between basePosition and nextPosition, based on t
    transform.position = Vector3.Lerp(basePosition, nextPosition, t);
    transform.rotation = Quaternion.Slerp(baseRotation, nextRotation, t);
  }

  bool IsPlayerRaycast(){
    var origin = head.position;
    var direction = (target.head.position - origin).normalized;
    Ray ray = new Ray(origin, direction);

    RaycastHit hit;
    if (Physics.Raycast(ray, out hit, float.MaxValue, targetLayerMask, QueryTriggerInteraction.Collide)){
      // testing hit on target player
      // slow? probably
      var parent = hit.transform.GetComponentInParent<PlayerEntity>();
      return parent == target;
    }
    return false;
  }

  public override void Serialize(ExitGames.Client.Photon.Hashtable h) {
    base.Serialize(h);

    h.Add('r', transform.rotation);
    h.Add('t', target ? target.authorityID : -1);
  }

  public override void Deserialize(ExitGames.Client.Photon.Hashtable h) {
    base.Deserialize(h);

    object val;
    // Accessing the Quaternion with the key 'r'
    if (h.TryGetValue('r', out val)){
      baseRotation = transform.rotation;
      nextRotation = (Quaternion)val;
    }

    if (h.TryGetValue('t', out val)){
      var id = (int)val;
      if (id >= 0){
        target = UnitManager.Local.GetPlayerEntity(id);
      } else {
        target = null;
      }
    }
  }

    /// <summary>
  /// Destroys ai if you own it. Otherwise disables it.
  /// </summary>
  public void DestroyAI() {
    if (isMine){
      // Deregister ai so it can STOP appearing on other people's clients
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
