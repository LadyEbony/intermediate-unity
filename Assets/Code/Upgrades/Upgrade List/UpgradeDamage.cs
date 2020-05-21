using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeDamage : Upgrade {

  public override void OnActivate() {
    var player = UnitManager.LocalPlayer;
    if (player){
      player.gun.damage = Mathf.RoundToInt(player.gun.damage * 1.25f);
    }
  }

}
