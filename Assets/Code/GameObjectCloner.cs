using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectCloner : MonoBehaviour {

  public GameObject target;
  public int count = 100;
  public float range = 5f;

  // Start is called before the first frame update
  void Start() {
    var b = target.transform.position;
    for(var i = 0; i < count; ++i){
      Instantiate(target, b + Random.insideUnitSphere * range, Quaternion.identity);
    }
  }
}
