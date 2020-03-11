using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogController : MonoBehaviour
{
    private float movementSpeed = 2.5f;
    Animation animator;
    Rigidbody rb;

    void Start()
    {
        animator = GetComponent<Animation>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        ControllPlayer();
    }

    void ControllPlayer()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        if (moveHorizontal == 2)
        {
            moveHorizontal = -1;
        }
        if (moveVertical == 2)
        {
            moveVertical = -1;
        }

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        if (movement != Vector3.zero)
        {
            if (moveVertical != -1)
            {
                animator.CrossFade("run");
                animator["run"].speed = 2f;
            }
            if (moveHorizontal != 0)
            {
                transform.Rotate(0, moveHorizontal * 2, 0, Space.World);
            }

        }
        else
        {
            animator.CrossFade("stand");
        }
        if (moveVertical == 1)
        {
            transform.Translate(movement * movementSpeed * Time.deltaTime);
        }
        else if (moveVertical == -1)
        {
            animator.CrossFade("stand");
        }
    }
}