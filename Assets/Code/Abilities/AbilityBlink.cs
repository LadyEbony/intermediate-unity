using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBlink : Ability {

  public float blink;
  public LayerMask layerMask;

  public override void Activate() {    
    var parent = GetComponentInParent<PlayerEntity>().transform;
    var position = parent.position;

    var rayorigin = Camera.main.transform.position;
    var raydirection = PlayerEntity.GetDirectionInput;

    // if no input, force it to go forward
    if (raydirection == Vector3.zero){
       raydirection = Camera.main.transform.forward;
       raydirection.y = 0;
       raydirection = raydirection.normalized;
    }

    RaycastHit hit;
    if(Physics.Raycast(rayorigin, raydirection, out hit, blink, layerMask)){
      // hits wall
      // use the X and Z of that wall
      position.x = hit.point.x;
      position.z = hit.point.z;
    } else {
      // doesn't hit wall
      // use the X and Z of the blink
      position += raydirection * blink; 
    }

    parent.position = position;
  }

}
