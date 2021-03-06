﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///Part is another serializable data storage class just like WeaponDefinition
///</summary>

[System.Serializable]

public class Part
{
    //These three fields need to be defined in the Inspector pane

    public string name;

    public float health;

    public string[] protectedBy;

    //These two fiedds are set automatically in Start().
    //Caching like this makes it faster and easier to find these later

    [HideInInspector]

    public GameObject go;

    [HideInInspector]

    public Material mat;
}

/// <summary>
/// Enemy_4 will start offscreen and then pick a random point on screen to
/// move to. Once it has arrived, it will pick another random point and
/// continue until the player has shot it down.
/// </summary>

public class Enemy_4 : Enemy
{
    [Header("Set in Inspector: Enemy_4")]

    public Part[] parts;

    public Vector3 p0, p1;

    private float timeStart;

    private float duration = 4;

    void Start()
    {
        //There is already an initial position chosen by Main.SpawnEnemy()
        //So add it to points as the initial p0 & p1.

        p0 = p1 = pos;

        InitMovement();

        //Cache GameObject & Material of each Part in parts

        Transform t;

        foreach (Part prt in parts)
        {
            t = transform.Find(prt.name);

            if(t != null)
            {
                prt.go = t.gameObject;

                prt.mat = prt.go.GetComponent<Renderer>().material;
            }
        }
    }

    void InitMovement()
    {
        p0 = p1;

        //Assign a new on-screen location to p1.

        float widMinRad = bndCheck.camWidth - bndCheck.radius;

        float hgtMidRad = bndCheck.camHeight - bndCheck.radius;

        p1.x = Random.Range(-widMinRad, widMinRad);

        p1.y = Random.Range(-hgtMidRad, hgtMidRad);

        //Reset the time

        timeStart = Time.time;
    }

    public override void Move()
    {
        //This completely overrides Enemy.Move() with a linear interpolation

        float u = (Time.time - timeStart) / duration;

        if (u >= 1)
        {
            InitMovement();

            u = 0;
        }

        u = 1 - Mathf.Pow(1 - u, 2);

        pos = (1 - u) * p0 + u * p1;
    }

    Part FindPart(string n)
    {
        foreach(Part prt in parts)
        {
            if (prt.name == n)
            {
                return (prt);
            }
        }
        return (null);
    }

    Part FindPart(GameObject go)
    {
        foreach(Part prt in parts)
        {
            if(prt.go == go)
            {
                return (prt);
            }
        }
        return (null);
    }

    //These fuctions return true if the Part has been destroyed

    bool Destroyed(GameObject go)
    {
        return (Destroyed(FindPart(go)));
    }

    bool Destroyed(string n)
    {
        return (Destroyed(FindPart(n)));
    }

    bool Destroyed(Part prt)
    {
        if (prt == null)
        {
            return (true);
        }

        //Returns the result of the comparison: prt.health <=0
        //If prt.health is 0 or less, returns true(yes, it was destroyed)

        return (prt.health <= 0);
    }

    //This changes the color of just one Part to red instead of the whole ship.

    void ShowLocalizedDamage(Material m)
    {
        m.color = Color.red;

        damageDoneTime = Time.time + showDamageDuration;

        showingDamage = true;
    }

    //This will override the OnCollisionEnter that is part of Enemy.cs

    void OnCollisionEnter(Collision coll)
    {
        GameObject other = coll.gameObject;

        switch (other.tag)
        {
            case "ProjectileHero":

                Projectile p = other.GetComponent<Projectile>();

                //If this Enemy is off screen, don't damage it.

            if (!bndCheck.isOnScreen)
                {
                    Destroy(other);

                    break;
                }

                //Hurt this Enemy

                GameObject goHit = coll.contacts[0].thisCollider.gameObject;

                Part prtHit = FindPart(goHit);

                if(prtHit == null)
                {
                    goHit = coll.contacts[0].otherCollider.gameObject;

                    prtHit = FindPart(goHit);
                }

                //Check whether this part is still protected

            if(prtHit.protectedBy != null)
                {
                    foreach(string s in prtHit.protectedBy)
                    {
                        //If one of the protecting parts hasn't been destroyed...

                        if (!Destroyed(s))
                        {
                          //...then don't damage this part yet

                          Destroy(other);

                          return;
                        }
                    }
                }

                //It's not protected, so make it take damage

                //Get the damage amount from the Projectile.type and Main.W_DEFS

                prtHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit;

                //Show damage on the part

                if (prtHit.health <= 0)
                {
                    //Instead of destroying this enemy, disable the damage part

                    prtHit.go.SetActive(false);
                }

                //Check to see if the whole ship is destroyed

                bool allDestroyed = true;

                foreach(Part prt in parts)
                {
                    if (!Destroyed(prt))
                    {
                        allDestroyed = false;

                        break;
                    }
                }

                if (allDestroyed)
                {
                    //..Tell the main singleton that this ship was destroyed

                    Main.S.ShipDestroyed(this);

                    //Destroy this Enemy

                    Destroy(this.gameObject);
                }

                Destroy(other);

                break;
        }
    }
}
