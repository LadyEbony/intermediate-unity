using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSpeed : Upgrade {

  public override void OnActivate() {
    var player = UnitManager.LocalPlayer;
    if (player){
      player.maxSpeed *= 1.25f;
    }
  }

}
