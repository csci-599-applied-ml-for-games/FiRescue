using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject dog;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
        Vector3 dogloc = new Vector3(dog.transform.position.x, 3, dog.transform.position.z);
        Vector3 viewPos = GetComponent<Camera>().WorldToViewportPoint(dog.transform.position);
        transform.LookAt(dogloc);
        Vector3 dogpos = new Vector3(dog.transform.position.x + 8, 3f, dog.transform.position.z);
        transform.position = Vector3.Lerp(transform.position, dogpos, 0.5f * Time.deltaTime);
        /*
        if (!(viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1 && viewPos.z > 0))
        {
            transform.LookAt(dogloc);
            //Vector3 dogpos = new Vector3(dog.transform.position.x + 3, 3f, dog.transform.position.z);
            //transform.position = Vector3.Lerp(transform.position, dogpos, 0.5f * Time.deltaTime);
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
        Vector3 dogloc = new Vector3(dog.transform.position.x, 3, dog.transform.position.z);
        Vector3 viewPos = GetComponent<Camera>().WorldToViewportPoint(dog.transform.position);
        transform.LookAt(dogloc);
        Vector3 dogpos = new Vector3(dog.transform.position.x + 8, 3f, dog.transform.position.z);
        transform.position = Vector3.Lerp(transform.position, dogpos, 0.5f * Time.deltaTime);
        /*
        if (!(viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1 && viewPos.z > 0))
        {
            transform.LookAt(dogloc);
            //Vector3 dogpos = new Vector3(dog.transform.position.x + 3, 3f, dog.transform.position.z);
            //transform.position = Vector3.Lerp(transform.position, dogpos, 0.5f * Time.deltaTime);
        }
        */
    }
}
