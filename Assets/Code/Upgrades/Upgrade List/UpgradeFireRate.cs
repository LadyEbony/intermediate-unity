using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeFireRate : Upgrade {

  public override void OnActivate() {
    var player = UnitManager.LocalPlayer;
    if (player){
      player.gun.fireRate *= 1.5f;
      player.gun.bulletSpeed *= 1.5f;
      player.gun.maxAmmoCount = (int)(player.gun.maxAmmoCount * 1.5f);
      player.gun.ammoCount = player.gun.maxAmmoCount;
    }
  }

}
