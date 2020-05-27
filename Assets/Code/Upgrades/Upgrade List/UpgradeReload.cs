using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeReload : Upgrade {
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
        var hitColliders = Physics.OverlapSphere(gun.transform.position, 50f, LayerMask.NameToLayer("Character"), QueryTriggerInteraction.Collide);
        foreach (var c in hitColliders)
        {
            var entity = c.transform.GetComponentInParent<CharacterEntity>();
            if (entity)
            {
                UnitManager.Local.RaiseEvent('d', true, entity.entityID, (byte)100, (byte)DamageType.Fire);
            }
        }
    }
}
