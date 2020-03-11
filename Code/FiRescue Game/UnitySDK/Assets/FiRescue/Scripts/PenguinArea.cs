using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using TMPro;
using System;

public class PenguinArea : Area
{
    public PenguinAgent penguinAgent;
    public GameObject safeZone;
    public Animal savedPrefabSquirrel;
    public Animal savePrefabSquirrel;
    public Animal savedPrefabRabbit;
    public Animal savePrefabRabbit;
    public TextMeshPro cumulativeRewardText;
    
    [HideInInspector]
    public float feedRadius = 1f;

    private List<GameObject> saveList;
    private List<GameObject> savedList;

    public AudioSource babySound;
    public AudioSource penguinSound;


    public override void ResetArea()
    {
        RemoveAllAnimals();
        PlaceAgent();
        PlaceSafeZone();
        SpawnAnimals(10);
    }

    public void RemoveSpecificAnimal(GameObject animalObject)
    {
        saveList.Remove(animalObject);
        Destroy(animalObject);
    }

    public void SaveAnimal(Vector3 pos, int i)
    {
        GameObject saveObject;
        if (i == 0)
        {
            saveObject = Instantiate<GameObject>(savedPrefabRabbit.gameObject);
        }
        else
        {
            saveObject = Instantiate<GameObject>(savedPrefabSquirrel.gameObject);
        }
        saveObject.transform.position = pos;
        saveObject.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
        savedList.Add(saveObject);
    }

    public static Vector3 ChooseRandomPosition(Vector3 center, float minAngle, float maxAngle, float minRadius, float maxRadius)
    {
        Vector3 mult = new Vector3(1, 0, 1);
        float radius = minRadius;

        if (maxRadius > minRadius)
        {
            radius = UnityEngine.Random.Range(minRadius, maxRadius);
        }

        return center + Quaternion.Euler(0f, UnityEngine.Random.Range(minAngle, maxAngle), 0f) * Vector3.forward * radius;
    }

    private void RemoveAllAnimals()
    {
        if (saveList != null)
        {
            for (int i = 0; i < saveList.Count; i++)
            {
                if (saveList[i] != null)
                {
                    Destroy(saveList[i]);
                }
            }
        }

        if (savedList != null)
        {
            for (int i = 0; i < savedList.Count; i++)
            {
                if (savedList[i] != null)
                {
                    Destroy(savedList[i]);
                }
            }
        }

        saveList = new List<GameObject>();
        savedList = new List<GameObject>();
    }

    private void PlaceAgent()
    {
        penguinAgent.transform.position = ChooseRandomPosition(transform.position, -45f, 45f, 4f, 9f) + Vector3.up;
        penguinAgent.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
    }

    private void PlaceSafeZone()
    {
        safeZone.transform.position = ChooseRandomPosition(transform.position, -45f, 45f, 4f, 9f) + Vector3.up*0.5f;
        //safeZone.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    private void SpawnAnimals(int count)
    {
        for (int i = 0; i < count/2; i++)
        {
            GameObject saveObject = Instantiate<GameObject>(savePrefabSquirrel.gameObject);
            saveObject.transform.position = ChooseRandomPosition(transform.position, 100f, 260f, 2f, 13f) + Vector3.up;
            saveObject.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
            saveObject.transform.parent = transform;
            saveList.Add(saveObject);
        }
        for (int i = 0; i < count / 2; i++)
        {
            GameObject saveObject = Instantiate<GameObject>(savePrefabRabbit.gameObject);
            saveObject.transform.position = ChooseRandomPosition(transform.position, 100f, 260f, 2f, 13f) + Vector3.up;
            saveObject.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
            saveObject.transform.parent = transform;
            saveList.Add(saveObject);
        }
    }

    private void Update()
    {
        cumulativeRewardText.text = penguinAgent.GetCumulativeReward().ToString("0.00");
    }
}
