using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeReload : Upgrade {
  
  public LayerMask explosionLayerMask;

    public override void OnActivate()
    {
        var player = UnitManager.LocalPlayer;
        if (player)
        {
            player.gun.onGunReloaded += OnGunReloadedTWO;
        }
    }

    private void OnGunReloadedTWO(Gun gun)
    {
        var hitColliders = Physics.OverlapSphere(gun.transform.position, 12f, explosionLayerMask, QueryTriggerInteraction.Collide);
        foreach (var c in hitColliders)
        {
            var entity = c.transform.GetComponentInParent<CharacterEntity>();
            if (entity && !(entity is PlayerEntity))
            {
                UnitManager.Local.RaiseEvent('d', true, entity.entityID, (byte)50, (byte)DamageType.Pure);
            }
        }
    }
}
