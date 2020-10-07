using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy_4 will start offscreen and then pick a random point on screen to
/// move to. Once it has arrived, it will pick another random point and
/// continue until the player has shot it down.
/// </summary>

public class Enemy_4 : Enemy
{
    public Vector3 p0, p1;

    private float timeStart;

    private float duration = 4;

    void Start()
    {
        //There is already an initial position chosen by Main.SpawnEnemy()
        //So add it to points as the initial p0 & p1.

        p0 = p1 = pos;

        InitMovement();
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
}
