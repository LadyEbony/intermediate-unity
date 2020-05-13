using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISpawner : MonoBehaviour {

  public float timer = 2f;
  private float nextTimer;

  private void Update() {
    if (NetworkManager.isMaster && GameInitializer.Instance.initialized && Time.time >= nextTimer){
      
      var entity = AIEntity.CreateEntity() as AIEntity;

      entity.nextPosition = transform.position;

      UnitManager.Local.Register(entity);

      nextTimer = Time.time + timer;
    }
  }
}
