using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

  // Start is called before the first frame update
  void Start() {
        
  }

  // Update is called once per frame
  void Update() {
    HandleMouse();
  }

  void HandleMouse(){
    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    transform.rotation = Quaternion.LookRotation(ray.direction, Vector3.up);
  }

}
