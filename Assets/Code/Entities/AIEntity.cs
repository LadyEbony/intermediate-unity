﻿using System.Collections;
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
  private float baseSpeed;
  private float slowSpeed;

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
    baseSpeed = ai.speed;
    slowSpeed = ai.speed * targetingSpeedModifier;

    if (isMine){
      
    } else {
      // just like how the rigidbody is disabled for other clients
      // same applies here
      Destroy(ai);
    }
  }

  public override void UpdateEntity() {
    base.UpdateEntity();

    // If ai targeting local client and within sight, shoot
    if (target && IsPlayerRaycast() && Time.time >= timeToFire){
      var bullet = BulletEntity.CreateEntity() as BulletEntity;

      bullet.startingPosition = barrelTransform.position;
      bullet.startingRotation = Quaternion.LookRotation((target.head.position - barrelTransform.position).normalized);
      bullet.baseDamage = bulletDamage;
      bullet.moveSpeed = bulletSpeed;
      bullet.timer = bulletAliveTime;
      bullet.reflection = 0;

      UnitManager.Local.RegisterLocal(bullet);

      timeToFire = Time.time + 1f / bulletFirerate;
    }
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
      ai.speed = IsPlayerRaycast() ? slowSpeed : baseSpeed;
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
      var layer = hit.transform.gameObject.layer;
      if (layer == 10) return false;  // hitting wall
      if (layer == 12) return hit.transform.parent.gameObject == target.gameObject; // hitting damage trigger
      return hit.transform.gameObject == target.gameObject; // hitting rigidbody ?????
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

}
