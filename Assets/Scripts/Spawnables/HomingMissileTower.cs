﻿using Photon.Pun;
using System.IO;
using UnityEngine;

public class HomingMissileTower : DefensiveBase
{
    public Transform nozzle;
    public override void PerformAttack()
    {
        if (opponentsInVicinity.Count < 1)
        {
            return;
        }

        //Projectile h = Instantiate(projectile, nozzle.position, nozzle.rotation);
        Projectile h = PhotonNetwork.Instantiate(Path.Combine(projectile.pathStrings), nozzle.position, nozzle.rotation, 0).GetComponent<Projectile>();
        h.GetComponent<HomingMissile>().SetTarget(opponentsInVicinity[0]);
        StartCoroutine(AttackAgainAfter(frequency));
    }

    public override void StopAttack()
    {
        if (opponentsInVicinity.Count > 0)
        {

        }
    }

}