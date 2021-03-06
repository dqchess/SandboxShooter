﻿using Photon.Pun;
using System.Collections;
using UnityEngine;

public class EnergyPack : MonoBehaviour
{
    public Rigidbody rBody;
    public Collider pickupCollider;

    public Material energyMaterial;

    public GameObject rotator;
    public GameObject nozzle;
    public GameObject masterBeam;
    public GameObject[] beams;
    public Color activeColor;
    public Color inactiveColor;
    
    [HideInInspector]
    public Camera playerCam;

    [HideInInspector]
    public bool isShooting;
    
    private bool hasEnergyWeapon;
    private Transform owner;
    private EnergyWeaponBase currentEnergyWeapon;
    private EPlayerController player;

    private PhotonView pView;

    private void Awake()
    {
        pView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        energyMaterial = new Material(energyMaterial);
        GetComponent<Renderer>().material = energyMaterial;

        energyMaterial.SetColor("_Color", activeColor);
    }

    private void OnEnable()
    {
        owner = transform.parent.root;
        player = owner.GetComponent<EPlayerController>();
        playerCam = Camera.main;
        isShooting = false;
        hasEnergyWeapon = false;
        DisableAttackBeam();
    }

    void Update()
    {
        if (!owner || !pView.IsMine)
        {
            return;
        }
        if(Input.GetButtonDown("Fire1") && player.IsInActiveGameplay())
        {
            if (player.playerEnergy.hasEnergyPack)
            {
                if (hasEnergyWeapon)
                {
                    if (currentEnergyWeapon.requiresBeams)
                    {
                        EnableAttackBeam(currentEnergyWeapon.beamsRequired);
                    }

                    currentEnergyWeapon.ShootEnergyWeapon();
                }
                else
                {
                    player.playerUI.DisplayAlertMessage("Energy weapon not equipped");
                }
            }
            else
            {
                player.playerUI.DisplayAlertMessage("Can't shoot without energy pack");
            }
        }

        if(isShooting && Input.GetButton("Fire1"))
        {
            if (currentEnergyWeapon.requiresBeams)
            {
                rotator.transform.SetPositionAndRotation(rotator.transform.position, playerCam.transform.rotation);
            }
        }

        if (isShooting && Input.GetButtonUp("Fire1"))
        {
            DisableAttackBeam();
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (!owner || ! pView.IsMine)
        {
            return;
        }
        if (other.GetComponent<PlayerEnergy>() && other.GetComponent<PlayerEnergy>().transform == owner)
        {
            //other.GetComponent<PlayerEnergy>().PickupEnergyPack(this);
            other.GetComponent<PlayerEnergy>().PickupEnergyPack(GetComponent<PhotonView>().ViewID);
        }
    }

    public void WasDropped()
    {
        pView.RPC("RPC_WasDropped", RpcTarget.All);
    }

    [PunRPC]
    void RPC_WasDropped()
    {
        DisableAttackBeam();
        energyMaterial.SetColor("_Color", inactiveColor);
        rBody.isKinematic = false;
        StartCoroutine(Countdown());
    }

    public void DisableAttackBeam()
    {
        pView.RPC("RPC_DisableAttackBeam", RpcTarget.All);
    }

    [PunRPC]
    void RPC_DisableAttackBeam()
    {
        isShooting = false;
        foreach (GameObject curr in beams)
        {
            curr.SetActive(false);
        }
        masterBeam.SetActive(false);

    }
    public void EnableAttackBeam(int beamNo)
    {
        pView.RPC("RPC_EnableAttackBeam", RpcTarget.All, beamNo);
    }

    [PunRPC]
    void RPC_EnableAttackBeam(int beamNo)
    {
        isShooting = true;
        masterBeam.SetActive(true);
        for (int loop = 0; loop <= (beamNo - 1); loop++)
        {
            beams[loop].SetActive(true);
        }
        //currentEnergyWeapon.ShootEnergyWeapon();
    }

    public void SetEnergyWeapon(EnergyWeaponBase weapon)
    {
        hasEnergyWeapon = true;
        currentEnergyWeapon = weapon;
        weapon.SetOwner(player);
        weapon.SetEnergySource(this);
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(5);
        energyMaterial.SetColor("_Color", activeColor);
        pickupCollider.enabled = true;
    }
}
