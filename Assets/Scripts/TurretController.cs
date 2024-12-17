using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    [SerializeField] private GameObject Bullet;
    
    [SerializeField] private float ShootTime, ReloadTime, BulletSpeed, BulletLifeTime, TimeBetweenBullets;
    
    private float ShootTimer, ReloadTimer;

    private bool notReloading,shooting;

    public bool deactivated;

    private Vector3 dir = Vector3.right;

    private void Start()
    {
        ShootTimer = ShootTime;
        ReloadTimer = ReloadTime;
        BulletSpeed *= 100;
        notReloading = true;
    }

    private void Update()
    {
        if (deactivated) return;
        if (notReloading)
        {
            Shooting();
        }
        else
        {
            Reloading();
        }
        
    }

    private void Shooting()
    {
        if (ShootTimer > 0)
        {
            ShootTimer -= Time.deltaTime;
            if (!shooting)StartCoroutine(Shoot());

        }
        else
        {
            notReloading = false;
            ShootTimer = ShootTime;
        }
        
    }


    IEnumerator Shoot()
    {
        shooting = true;
        yield return new WaitForSeconds(TimeBetweenBullets);
        var bul = Instantiate(Bullet,transform.position,quaternion.identity);
        bul.GetComponent<Rigidbody>().AddForce(dir*BulletSpeed);
        
        var bc = bul.GetComponent<BulletController>();
        var ic = GetComponent<ItemController>();
            bc.isGhost = ic.isGhost;
        bc.isP1 = ic.isP1;
        shooting = false;
        Destroy(bul,BulletLifeTime);
        
    }

    private void Reloading()
    {
        if (ReloadTimer > 0)
        {
            ReloadTimer -= Time.deltaTime;
        }
        else
        {
            notReloading = true;
            ReloadTimer = ReloadTime;
        }
    }

    private void RoundReset()
    {
        deactivated = true;
        ShootTimer = ShootTime;
        ReloadTimer = ReloadTime;
        notReloading = true;
        
    }

    public void Deactivate()
    {
        deactivated = true;
        ShootTimer = ShootTime;
        ReloadTimer = ReloadTime;
        notReloading = true;
    }
    
    public void Activate()
    {
        deactivated = false;
        ShootTimer = ShootTime;
        ReloadTimer = ReloadTime;
        notReloading = true;
    }

    public void DirectionRight(bool r)
    {
        if (r)
        {
            dir = Vector3.right;
        }
        
        if (r)
        {
            dir = -Vector3.right;
        }
    }
}
