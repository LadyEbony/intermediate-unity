using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeBulletMultiply : Upgrade {

  public override void OnActivate() {
    var player = UnitManager.LocalPlayer;
    if (player){
      player.gun.onBulletFired += OnBulletFiredTWO;
    }
  }

  private void OnBulletFiredTWO(Gun gun){
    gun.ammoCount += 1;
  }

}
