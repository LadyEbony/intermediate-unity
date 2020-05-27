using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeFireBullets : Upgrade {

  public override void OnActivate() {
    var player = UnitManager.LocalPlayer;
    if (player){
      player.gun.fireAmmoModifer = 200;
      player.gun.poisonAmmoModifer = 75;
    }
  }

}
