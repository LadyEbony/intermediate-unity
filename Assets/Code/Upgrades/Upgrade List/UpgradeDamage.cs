using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeDamage : Upgrade {

  public override void OnActivate() {
    var player = UnitManager.LocalPlayer;
    if (player){
      player.gun.damage = (int)(player.gun.damage * 1.5f);
      player.gun.reloadTime *= 0.5f;
    }
  }

}
