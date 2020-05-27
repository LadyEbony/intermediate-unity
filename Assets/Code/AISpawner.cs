using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISpawner : MonoBehaviour {

  public float randomStartTimer = 20f;
  public float randomEndTimer = 40f;
  public float timer {
    get {
      return Random.Range(randomStartTimer, randomEndTimer) / NetworkManager.net.CurrentRoom.PlayerCount;
    }
  }
  private float nextTimer;

  public float radius = 2f;
  public LayerMask layerMask;

  private void Update() {
    if (GameInitializer.Instance.initialized && nextTimer == 0f){
      nextTimer = Time.time + Random.Range(randomStartTimer, randomEndTimer) * 0.5f;
    }

    if (NetworkManager.isMaster && GameInitializer.Instance.initialized && Time.time >= nextTimer){
      
      if (Physics.CheckSphere(transform.position, radius, layerMask)) {
        nextTimer = Time.time + timer * 0.5f;
        return;
      }

      var entity = AIEntity.CreateEntity() as AIEntity;

      entity.nextPosition = transform.position;

      UnitManager.Local.Register(entity);

      nextTimer = Time.time + timer;
    }
  }

  private void OnDrawGizmosSelected() {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, radius);
  }
}
