using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeFireRate : Upgrade {

  public override void OnActivate() {
    var player = UnitManager.LocalPlayer;
    if (player){
      player.gun.fireRate *= 1.25f;
    }
  }

}
