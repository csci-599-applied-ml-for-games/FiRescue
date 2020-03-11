using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System;

public class PenguinAgent : Agent
{
    public GameObject heartPrefab;

    private ForestArea forestArea;
    private Animation animator;
    private RayPerception3D rayPerception;
    private GameObject safeZone;
    private AudioSource babySound;
    private AudioSource penguinSound;
    private float movementSpeed = 2.5f;
    private int saved_id = 0;
    private int jump_second;
    private bool jumped = false;
    Rigidbody rb;

    private bool isFull; // If true, penguin has a full stomach

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        float moveHorizontal = vectorAction[1];
        float moveVertical = vectorAction[0];
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
                transform.Rotate(0, moveHorizontal*2, 0, Space.World);
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
        // Tiny negative reward every step
        AddReward(-1f / agentParameters.maxStep);
    }

    public override void AgentReset()
    {
        isFull = false;
        forestArea.ResetArea();
    }

    public override void CollectObservations()
    {
        // Has the penguin eaten
        AddVectorObs(isFull);

        // Distance to the safe zone
        AddVectorObs(Vector3.Distance(safeZone.transform.position, transform.position));

        // Direction to safe zone
        AddVectorObs((safeZone.transform.position - transform.position).normalized);

        // Direction penguin is facing
        AddVectorObs(transform.forward);

        // RayPerception (sight)
        // ========================
        // rayDistance: How far to raycast
        // rayAngles: Angles to raycast (0 is right, 90 is forward, 180 is left)
        // detectableObjects: List of tags which correspond to object types agent can see
        // startOffset: Starting height offset of ray from center of agent
        // endOffset: Ending height offset of ray from center of agent
        float rayDistance = 20f;
        float[] rayAngles = { 30f, 60f, 90f, 120f, 150f };
        string[] detectableObjects = { "safeZone", "saveRabbit", "saveSquirrel", "wall" };
        AddVectorObs(rayPerception.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
    }

    private void Start()
    {
        forestArea = GetComponentInParent<ForestArea>();
        animator = GetComponent<Animation>();
        rayPerception = GetComponent<RayPerception3D>();
        babySound = forestArea.animalSound;
        penguinSound = forestArea.dogSound;
        rb = GetComponent<Rigidbody>();
        safeZone = forestArea.safeZone;
        safeZone.GetComponent<Animation>().CrossFade("idle");
        saved_id = 0;
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, safeZone.transform.position) < forestArea.feedRadius)
        {
            // Close enough, try to save the animal
            DropAnimal(saved_id);
        }
        if (jumped && System.DateTime.Now.Second - jump_second >= 2)
        {
            safeZone.GetComponent<Animation>().Play();
            jumped = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("saveRabbit"))
        {
            saved_id = 0;
            PickAnimal(collision.gameObject);
        }
        else if (collision.transform.CompareTag("saveSquirrel"))
        {
            saved_id = 1;
            PickAnimal(collision.gameObject);
        }
        else if (collision.transform.CompareTag("safeZone"))
        {
            DropAnimal(saved_id);
        }
    }

    private void PickAnimal(GameObject animalObject)
    {
        if (isFull) return; // Can't save another animal while full
        isFull = true;

        forestArea.RemoveSpecificAnimal(animalObject);

        AddReward(2.5f);
    }

    private void DropAnimal(int i)
    {
        if (!isFull)
        {
            return; // Nothing to save
        }
        jumped = true;
        isFull = false;
        // Spawn heart
        GameObject heart = Instantiate<GameObject>(heartPrefab);
        heart.transform.parent = transform.parent;
        heart.transform.position = safeZone.transform.position + Vector3.up;
        safeZone.GetComponent<Animation>()["jump"].speed = 0.3f;
        safeZone.GetComponent<Animation>().CrossFade("jump");
        jump_second = System.DateTime.Now.Second;
        Destroy(heart, 4f);
        babySound.Play();
        forestArea.SaveAnimal(transform.position, i);
        AddReward(1.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        penguinSound.Play();
        AddVectorObs(Vector3.Distance(other.transform.position, transform.position));
        AddReward(-4f);
    }
}
