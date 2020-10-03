using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is an enum of the possible weapon types
//It also includes a "shield" type to allow a shield power-up.
//Items marked [NI] beelow are Not Implemented in the IGPD Book

public enum WeaponType
{
    none,   //The default/no weapon

    blaster,    //A simple blaster

    spread,     //Two shots simultaneously

    phaser,     //[NI] Shots that move in waves

    missile,        //[NI] Homing missiles

    laser,      //[NI] Damage over time
    
    shield      //Raise shieldLevel
}

public class Weapon: MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
