using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeBulletMultiply : Upgrade {

  public override void OnActivate() {
    var player = UnitManager.LocalPlayer;
    if (player){
      player.gun.onBulletFired += OnBulletFiredTWO;
      player.gun.angleDeviation *= 4f;
    }
  }

  private void OnBulletFiredTWO(Gun gun){
    if (Random.value <= 0.5f)
      gun.Shoot();
    else
      gun.ammoCount += 1;
  }

}
