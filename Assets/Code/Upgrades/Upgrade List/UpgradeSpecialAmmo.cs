using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSpecialAmmo : Upgrade {

  public override void OnActivate() {
    var player = UnitManager.LocalPlayer;
    if (player){
      player.gun.effectTimer += 3;
    }
  }
}
