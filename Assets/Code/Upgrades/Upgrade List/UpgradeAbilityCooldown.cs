using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeAbilityCooldown : Upgrade {

  public override void OnActivate() {
    var player = UnitManager.LocalPlayer;
    if (player){
      player.mainAbility.cooldown *= 0.5f;
      player.alternateAbility.cooldown *= 0.5f;
    }
  }
 
}
