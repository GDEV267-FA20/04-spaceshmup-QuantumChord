﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary
///This is an enum of the possible weapon types
///It also includes a "shield" type to allow a shield power-up.
///Items marked [NI] beelow are Not Implemented in the IGPD Book
///</summary>

public enum WeaponType
{
    none,   //The default/no weapon

    blaster,    //A simple blaster

    spread,     //Two shots simultaneously

    phaser,     //Shots that move in waves

    missile,        //[NI] Homing missiles

    laser,      //[NI] Damage over time
    
    shield      //Raise shieldLevel
}

///<summary
///The WeaponDefinition class alows you to set the properties
///of a specific weapon in the Inspector. The main class has
///an array of WeaponDefinitions that makes this possible.
///</summary>

[System.Serializable]

public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;

    public string letter;

    public Color color = Color.white;

    public GameObject projectilePrefab;

    public Color projectileColor = Color.white;

    public float damageOnHit = 0;

    public float continuousDamage = 0;

    public float delayBetweenShots = 0;

    public float velocity = 20;
}

public class Weapon: MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;

    [Header("Set Dynamically")] [SerializeField]

    private WeaponType _type = WeaponType.none;

    public WeaponDefinition def;

    public GameObject collar;

    public float lastShotTime;

    private Renderer collarRend;

    void Start()
    {
        collar = transform.Find("Collar").gameObject;

        collarRend = collar.GetComponent<Renderer>();

        //Call SetType() for the default_type of WeaponType.none

        SetType(_type);

        //Dynamically create an anchor for all Projectiles

        if (PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");

            PROJECTILE_ANCHOR = go.transform;
        }

        //Find the fireDelegate of the root GameObject

        GameObject rootGO = transform.root.gameObject;

        if (rootGO.GetComponent<Hero>() != null)
        {
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
        }
    }

    public WeaponType type
    {
        get { return (_type); }

        set { SetType(value); }
    }

    public void SetType(WeaponType wt)
    {
        _type = wt;

        if (type == WeaponType.none)
        {
            this.gameObject.SetActive(false);

            return;
        }

        else
        {
            this.gameObject.SetActive(true);
        }

        def = Main.GetWeaponDefinition(_type);

        collarRend.material.color = def.color;

        lastShotTime = 0;
    }

    public void Fire()
    {
        //If this.gameObject is inactive, return

        if (!gameObject.activeInHierarchy) return;

        //If it hasn't been enough time ebtween shots, return

        if(Time.time - lastShotTime < def.delayBetweenShots)
        {
            return;
        }

        Projectile p;

        Vector3 vel = Vector3.up * def.velocity;

        if (transform.up.y < 0)
        {
            vel.y = -vel.y;
        }

        switch (type)
        {
            case WeaponType.blaster:

                p = MakeProjectile();

                p.rigid.velocity = vel;

                break;

            case WeaponType.spread:

                p = MakeProjectile();

                p.rigid.velocity = vel;

                p = MakeProjectile();

                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);

                p.rigid.velocity = p.transform.rotation * vel;

                p = MakeProjectile();

                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);

                p.rigid.velocity = p.transform.rotation * vel;

                break;

           /* case WeaponType.laser:

                p = MakeProjectile();

                p.rigid.velocity = vel;

                break;*/
        }
    }

    public Projectile MakeProjectile()
    {
        GameObject go = Instantiate<GameObject>(def.projectilePrefab);

        if(transform.parent.gameObject.tag == "Hero")
        {
            go.tag = "ProjectileHero";

            go.layer = LayerMask.NameToLayer("ProjectileHero");
        }

        else
        {
            go.tag = "ProjectileEnemy";

            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }

        go.transform.position = collar.transform.position;

        go.transform.SetParent(PROJECTILE_ANCHOR, true);

        Projectile p = go.GetComponent<Projectile>();

        p.type = type;

        lastShotTime = Time.time;

        return (p);
    }
}
