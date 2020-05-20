using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour {

  // NEW: Separate from AIHealthBar since we may want this for other gameobjects
  private void LateUpdate() {
    var cam = Camera.main.transform;
    transform.LookAt(transform.position + cam.rotation * Vector3.forward, cam.rotation * Vector3.up);
  }

}
