using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    [SerializeField] private GameObject Bullet;
    
    [SerializeField] private float ShootTime, ReloadTime, BulletSpeed, BulletLifeTime, TimeBetweenBullets;
    
    private float ShootTimer, ReloadTimer;

    private bool notReloading,shooting;

    private Vector3 dir = Vector3.right;

    private List<GameObject> bullets = new List<GameObject>();

    private void Start()
    {
        ShootTimer = ShootTime;
        ReloadTimer = ReloadTime;
        BulletSpeed *= 100;
        notReloading = true;
    }

    private void Update()
    {
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
            if (!shooting && ShootTimer < ShootTime)StartCoroutine(Shoot());

        }
        else
        {
            notReloading = false;
            ShootTimer = ShootTime;
        }
        
    }


    IEnumerator Shoot()
    {
        if (ShootTimer < ShootTime)
        {
            shooting = true;
            yield return new WaitForSeconds(TimeBetweenBullets);
            var bul = Instantiate(Bullet,transform.position,quaternion.identity);
            bullets.Add(bul);
            bul.GetComponent<Rigidbody>().AddForce(dir*BulletSpeed);
            var bc = bul.GetComponent<BulletController>();
            var ic = GetComponent<ItemController>();
            bc.isGhost = ic.isGhost;
            bc.isP1 = ic.isP1;
            shooting = false;
            yield return new WaitForSeconds(BulletLifeTime);
            bullets.Remove(bul);
            Destroy(bul);
        }
        
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
        ShootTimer = ShootTime;
        ReloadTimer = ReloadTime;
        notReloading = true;
        
    }

    public void Deactivate()
    {
        ShootTimer = ShootTime;
        ReloadTimer = ReloadTime;
        notReloading = true;
    }
    
    public void Activate()
    {
        ShootTimer = ShootTime;
        notReloading = true;
        ReloadTimer = ReloadTime;
        if (bullets == null) return;
        foreach (var GOB in bullets)
        {
            Destroy(GOB);
        }
        bullets.Clear();

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
