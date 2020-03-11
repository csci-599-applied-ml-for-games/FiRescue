using System.Collections;

using System.Collections.Generic;

using UnityEngine;



public class Animal : MonoBehaviour

{

    public float health = 100f;

    //Distance from fire

    float minDistanceToDanger = 5f;

    float CONSTANT_FIRE = 1f;

    float[] rayAngles = { 60f, 120f, 180f, 240f, 300f, 360f };

    string[] detectableObjects = { "fire" };

    //Called with every second passed in the simulation

    private RaycastHit hit;

    public Ray[] rays;



    private void Start()
    {

        for (int i = 0; i < rayAngles.Length; i++)
        {

            Quaternion spreadAngle = Quaternion.AngleAxis(rayAngles[i], new Vector3(0, 1, 0));
            Ray ray = new Ray(transform.position, spreadAngle * transform.forward);
            rays[i] = ray;

        }

        InvokeRepeating("UpdateHealth", 1.0f, 1.0f);

    }



    private float rayCast(Ray ray, RaycastHit hit)

    {

        float distance = 0f;

        if (Physics.Raycast(ray, out hit, minDistanceToDanger))
        {

            if (hit.collider.tag == "fire")

            {

                distance = Vector3.Distance(hit.point, transform.position);

                return distance;

                //Debug.Log("Hit distance = " + Vector3.Distance(hit.point, transform.position));

            }

        }

        return -1;

    }



    private void OnCollisionEnter(Collision other)
    {

        if (other.transform.CompareTag("fire"))
        {

            //Update Health and check for death condition

            health -= 5f;

        }

    }


    public float getHealth()
    {

        return health;

    }



    public void reset()
    {

        health = 100f;

    }



    public void UpdateHealth()
    {

        float min = 10000f;

        for (int i = 0; i < rays.Length; i++)
        {

            float distance = rayCast(rays[i], hit);

            if (distance == -1)
            {

                continue;

            }

            if (distance < min)

                min = distance;

            //Decrease health

            if (min != 10000f)
            {

                health -= CONSTANT_FIRE * (10 / min);

            }

        }

    }

}
