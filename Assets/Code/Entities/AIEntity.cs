using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.AI;

public class AIEntity : CharacterEntity {

  private NavMeshAgent ai;

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

  protected override void LocalUpdate() {
    base.LocalUpdate();

    // near destination
    if (Vector3.SqrMagnitude(transform.position - ai.destination) < 0.25f){
      float searchRange = 5f;
      while(searchRange < 100f){
        var random = Random.insideUnitSphere * searchRange;
        random.y = 0f;

        NavMeshHit hit;
        if(NavMesh.SamplePosition(transform.position + random, out hit, searchRange * 2f, NavMesh.AllAreas)){
          // check if the sampled position is a valid walking point
          ai.destination = hit.position;
          break;
        }

        searchRange *= 2f;
      }
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

  public override void Serialize(ExitGames.Client.Photon.Hashtable h) {
    base.Serialize(h);

    h.Add('r', transform.rotation);
  }

  public override void Deserialize(ExitGames.Client.Photon.Hashtable h) {
    base.Deserialize(h);

    object val;
    // Accessing the Quaternion with the key 'r'
    if (h.TryGetValue('r', out val)){
      baseRotation = transform.rotation;
      nextRotation = (Quaternion)val;
    }
  }

}
