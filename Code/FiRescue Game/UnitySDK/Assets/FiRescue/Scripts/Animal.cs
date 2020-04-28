using System.Collections;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;



public class Animal : MonoBehaviour
{
    //public float health = 100f;
    public Slider animalHealth;
    private float lastUpdate = 0.0f;
    RaycastHit hit_first;

    private void Start()
    {
        //health = 100f;
        animalHealth.value = 1;
        lastUpdate = System.DateTime.Now.Second;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit_first) && hit_first.transform.tag == "fire")
        {
            lastUpdate = System.DateTime.Now.Second;
            //health -= (1 / (hit_first.distance));
            animalHealth.value -= (hit_first.distance * 0.1f);
            //Debug.Log(lastUpdate + " - " + animalHealth.value);
        }
    }

    void Update()
    {
        RaycastHit hit;
        //Debug.Log(System.DateTime.Now.Second + " - last - " + lastUpdate);
        if ((System.DateTime.Now.Second - lastUpdate > 1) && Physics.Raycast(transform.position, -Vector3.up, out hit) && hit.transform.tag == "fire")
        {
            //health -= (1/(hit.distance));
            lastUpdate = System.DateTime.Now.Second;
            animalHealth.value -= (hit.distance * 0.5f);
            //Debug.Log(animalHealth.value);
        }
    }
}
