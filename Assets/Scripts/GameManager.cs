﻿using Photon.Pun;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Spawnable> offensiveSpawnables;
    public List<Spawnable> defensiveSpawnables;
    public List<Spawnable> decorativeSpawnables;

    void Start()
    {
        SetupUniversalCollisionsRules();
    }

    public void SpawnSpawnable(SpawnableType type, string id, Vector3 location, Quaternion rotation, EPlayerController owner)
    {
        Spawnable receivedBlueprint = GetBlueprint(type, id);
        
        //GameObject spawnedSpawnable = Instantiate(receivedBlueprint.prefab, location, rotation);
        GameObject spawnedSpawnable = PhotonNetwork.Instantiate(Path.Combine(receivedBlueprint.pathStrings), location, rotation, 0);
        spawnedSpawnable.GetComponent<SpawnableGO>().displayName = receivedBlueprint.displayName;
        spawnedSpawnable.GetComponentInChildren<SpawnableHealth>().InitiateSystems(receivedBlueprint.health);

        if (spawnedSpawnable.GetComponent<DefensiveBase>())
        {
            spawnedSpawnable.GetComponent<DefensiveBase>().SetOwner(owner);
        }
    }

    public void SpawnPlatform(string[] pathParts, Vector3 loc)
    {
        //GameObject spawnedSpawnable = Instantiate(receivedBlueprint.prefab, location, rotation);
        GameObject spawnedSpawnable = PhotonNetwork.Instantiate(Path.Combine(pathParts), loc, Quaternion.identity, 0);
    }


    public Spawnable GetBlueprint(SpawnableType type, string id)
    {
        switch (type)
        {
            case SpawnableType.Offence:
                foreach(Spawnable curr in offensiveSpawnables)
                {
                    if (curr.name == id)
                    {
                        return curr;
                    }
                }
                break;

            case SpawnableType.Defence:
                foreach (Spawnable curr in defensiveSpawnables)
                {
                    if (curr.name == id)
                    {
                        return curr;
                    }
                }
                break;

            case SpawnableType.Decoration:
                foreach (Spawnable curr in decorativeSpawnables)
                {
                    if (curr.name == id)
                    {
                        return curr;
                    }
                }
                break;
        }
        return new Spawnable();
    }

    public bool ObjectExistsAtLocation(Vector3 loc)
    {
        if(Physics.CheckSphere(loc, 1, (1 << LayerMask.NameToLayer("SpawnableGO"))))
        {
            return true;
        }
        return false;
    }

    public void SetupUniversalCollisionsRules()
    {
        //Ignore collision between bounds and projectiles
        Physics.IgnoreLayerCollision(10, 11);
        
        //Ignore collision between bounds and orbitals
        Physics.IgnoreLayerCollision(10, 12);
    }

    public Vector3 GetSpawnLocation()
    {
        SpawnPoint[] allPoints = GameObject.FindObjectsOfType<SpawnPoint>();
        if (allPoints.Length > 0)
        {
            return allPoints[Random.Range(0, (allPoints.Length - 1))].transform.position;
        }
        return new Vector3(0, 0, 0);
    }

    public EPlayerController FetchLocalPlayer()
    {
        foreach (EPlayerController curr in FindObjectsOfType<EPlayerController>())
        {
            if (curr.GetComponent<PhotonView>().IsMine)
            {
                return curr;
            }
        }
        return GameObject.FindObjectOfType<EPlayerController>();
    }

}