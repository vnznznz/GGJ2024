using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Person : MonoBehaviour
{
    private Vector3 startPosition;


    private Vector3 targetWobbleAmplitude;
    private Vector3 targetWobbleFrequency;

    private Vector3 currentWobbleFrequency;
    private Vector3 currentWobbleAmplitude;
    private Vector3 currentWobblePhase;

    private MeshRenderer meshRenderer;

    public float enjoymentValue;

    public enum BehaviorState
    {
        Neutral,
        Happy,
        Booing,
        Angry,
        Leaving
    }

    public BehaviorState behaviorState = BehaviorState.Neutral;

    public string[] audienceTags;

    void Start()
    {
        startPosition = transform.position + new Vector3(0, 1.5f, 0);
        currentWobblePhase = new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);

        meshRenderer = GetComponent<MeshRenderer>();

    }


    void Update()
    {
        UpdateBehavior();
        UpdateMovement();

        float enj = 1f - (Math.Clamp(this.enjoymentValue, 1, 100) / 100f);
        var col = new Color(1f, enj, 1f);
        meshRenderer.material.SetColor("_Color", col);
    }


    private void UpdateBehavior()
    {
        switch (behaviorState)
        {
            case BehaviorState.Neutral:
                UpdateNeutral();
                break;
            case BehaviorState.Happy:
                UpdateHappy();
                break;
            case BehaviorState.Booing:
                UpdateBooing();
                break;
            case BehaviorState.Angry:
                UpdateAngry();
                break;
            case BehaviorState.Leaving:
                UpdateLeaving();
                break;

        }
    }

    private void UpdateMovement()
    {
        currentWobbleAmplitude = Vector3.Lerp(currentWobbleAmplitude, targetWobbleAmplitude, Time.deltaTime * 0.1f);
        currentWobbleFrequency = Vector3.Lerp(currentWobbleFrequency, targetWobbleFrequency, Time.deltaTime * 0.1f);


        Vector3 pos = transform.position;
        Vector3 wobbleOffset = new Vector3(Mathf.Sin(currentWobblePhase.x) * currentWobbleAmplitude.x,
                                           Mathf.Sin(currentWobblePhase.y) * currentWobbleAmplitude.y,
                                           Mathf.Sin(currentWobblePhase.z) * currentWobbleAmplitude.z);

        transform.position = startPosition + wobbleOffset;

        currentWobblePhase += currentWobbleFrequency * (Time.deltaTime * Mathf.PI * 2);
    }

    private void UpdateNeutral()
    {
        targetWobbleAmplitude = new Vector3(0.01f, 0.05f, 0.01f);
        targetWobbleFrequency = new Vector3(0.5f, 0.5f, 0.5f);
    }
    private void UpdateHappy()
    {
        targetWobbleAmplitude = new Vector3(0.01f, 0.2f, 0.01f);
        targetWobbleFrequency = new Vector3(0.5f, 0.8f, 0.5f);
    }

    private void UpdateBooing()
    {
        targetWobbleAmplitude = new Vector3(0.05f, 0.2f, 0.05f);
        targetWobbleFrequency = new Vector3(0.8f, 1f, 0.8f);
    }

    private void UpdateAngry()
    {

    }

    private void UpdateLeaving()
    {

    }



}
