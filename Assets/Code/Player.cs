using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour {

  public NavMeshAgent nva;
  public float speed;

  // Update is called once per frame
  void Update() {
    var hor = Input.GetAxisRaw("Horizontal");
    var ver = Input.GetAxisRaw("Vertical");
    var steering = new Vector3(hor, 0f, ver);
    var delta = steering * speed * Time.deltaTime;
    nva.Move(delta);
  }
}
