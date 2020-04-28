﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using UnityEngine.UI;
using System;

public class DogAgent : Agent
{
    public GameObject heartPrefab;
    public Slider dogHealth;
    public Image fillBar;
    public ProgressBarCircle PbC;
    public GameObject HappyKoala;
    public GameObject SadKoala;
    public GameObject Squirrel;
    public GameObject Rabbit;

    private ForestArea forestArea;
    private Animation animator;
    private RayPerception3D rayPerception;
    private GameObject safeZone;
    private AudioSource animalSound;
    private AudioSource dogSound;
    private float movementSpeed = 3.0f;
    private int saved_id = 0;
    private int jump_second;
    private bool jumped = false;
    private float lastWallCollision=0.0f;
    private float lastSaved = 0.0f;
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
        // Tiny negative reward every step
        AddReward(-1f / agentParameters.maxStep);
    }

    public override void AgentReset()
    {
        isFull = false;
        PbC.BarValue = 0;
        lastSaved = System.DateTime.Now.Second;
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
        string[] detectableObjects = { "safeZone", "saveRabbit", "saveSquirrel", "savedRabbit", "savedSquirrel", "wall", "fire" };
        AddVectorObs(rayPerception.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
    }

    private void Start()
    {
        forestArea = GetComponentInParent<ForestArea>();
        animator = GetComponent<Animation>();
        rayPerception = GetComponent<RayPerception3D>();
        animalSound = forestArea.animalSound;
        dogSound = forestArea.dogSound;
        rb = GetComponent<Rigidbody>();
        safeZone = forestArea.safeZone;
        safeZone.GetComponent<Animation>().CrossFade("idle");
        saved_id = 0;
        PbC.BarValue = 0;
        SadKoala.SetActive(true);
        HappyKoala.SetActive(false);
        lastWallCollision = 0.0f;
        lastSaved = System.DateTime.Now.Second;
        Rabbit.SetActive(false);
        Squirrel.SetActive(false);
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
        if (dogHealth.value <= 0.3f)
        {
            fillBar.color = Color.red;
        }
        if (PbC.BarValue < 50)
        {
            SadKoala.SetActive(true);
            HappyKoala.SetActive(false);
        }
        if (PbC.BarValue >= 50)
        {
            SadKoala.SetActive(false);
            HappyKoala.SetActive(true);
        }
        if (dogHealth.value == 0)
        {
            Done();
        }
        /*
        if (System.DateTime.Now.Second - lastSaved >= 15)
        {
            lastSaved = System.DateTime.Now.Second;
            forestArea.PlaceAgent();
        }
        */
    }

    private void OnCollisionEnter(Collision collision)
    {
        string tag = collision.transform.tag;
        
        if (collision.transform.CompareTag("wall"))
        {
            if (lastWallCollision == 0.0f)
            {
                lastWallCollision = System.DateTime.Now.Second;
            }
            else if (System.DateTime.Now.Second - lastWallCollision >= 1.5 && System.DateTime.Now.Second - lastWallCollision <= 8)
            {
                lastWallCollision = System.DateTime.Now.Second;
                //Done();
                forestArea.PlaceAgent();
            }
            else if(System.DateTime.Now.Second - lastWallCollision > 8)
            {
                lastWallCollision = System.DateTime.Now.Second;
            }
        }
        
        if (collision.transform.CompareTag("saveRabbit"))
        {
            if (!isFull)
            {
                saved_id = 0;
            }
            PickAnimal(collision.gameObject);
            AddVectorObs(Vector3.Distance(collision.transform.position, transform.position));
        }
        else if (collision.transform.CompareTag("saveSquirrel"))
        {
            if (!isFull)
            {
                saved_id = 1;
            }
            PickAnimal(collision.gameObject);
            AddVectorObs(Vector3.Distance(collision.transform.position, transform.position));
        }
        else if (collision.transform.CompareTag("safeZone"))
        {
            DropAnimal(saved_id);
            AddVectorObs(Vector3.Distance(collision.transform.position, transform.position));
        }
    }

    private void PickAnimal(GameObject animalObject)
    {
        if (isFull) return; // Can't save another animal while full
        isFull = true;
        if (saved_id == 0)
        {
            Rabbit.SetActive(true);
        }
        else if (saved_id == 1)
        {
            Squirrel.SetActive(true);
        }
        forestArea.RemoveSpecificAnimal(animalObject);
        dogSound.Play();
        AddReward(3.5f);
        //AddReward(0.7f);
    }

    private void DropAnimal(int i)
    {
        if (!isFull)
        {
            return; // Nothing to save
        }
        jumped = true;
        Rabbit.SetActive(false);
        Squirrel.SetActive(false);
        isFull = false;
        // Spawn heart
        GameObject heart = Instantiate<GameObject>(heartPrefab);
        heart.transform.parent = transform.parent;
        heart.transform.position = safeZone.transform.position + Vector3.up;
        safeZone.GetComponent<Animation>()["jump"].speed = 0.3f;
        safeZone.GetComponent<Animation>().CrossFade("jump");
        jump_second = System.DateTime.Now.Second;
        Destroy(heart, 4f);
        animalSound.Play();
        forestArea.SaveAnimal(transform.position, i);
        AddReward(3.5f);
        //AddReward(0.5f);
        PbC.BarValue += 10;
        lastSaved = System.DateTime.Now.Second;
    }

    private void OnTriggerEnter(Collider other)
    {
        dogSound.Play();
        AddVectorObs(Vector3.Distance(other.transform.position, transform.position));
        AddReward(-4f);
        //AddReward(-1f);
        dogHealth.value -= 0.01f;
        //Done();
    }
}
