using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Person : MonoBehaviour
{
    private Vector3 startPosition;

    private Vector3 targetPosition;
    private Vector3 currentPosition;
    private float randomXOffset = 0;


    private Vector3 targetWobbleAmplitude;
    private Vector3 targetWobbleFrequency;

    private Vector3 currentWobbleFrequency;
    private Vector3 currentWobbleAmplitude;
    private Vector3 currentWobblePhase;


    public float enjoymentValue;

    private Transform leavingPoint;

    public enum BehaviorState
    {
        Neutral,
        Happy,
        Booing,
        Angry,
        Leaving
    }

    public BehaviorState behaviorState = BehaviorState.Neutral;

    void Start()
    {
        startPosition = transform.position;
        targetPosition = transform.position;
        currentPosition = transform.position;
        randomXOffset = Random.Range(0,0.5f);

        currentWobblePhase = new Vector3(Random.value, Random.value, Random.value);

        leavingPoint = GameObject.Find("LeavingPosition").transform;
    }


    void Update()
    {
        UpdateBehavior();
        UpdateMovement();
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
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * 2f);

        currentWobbleAmplitude = Vector3.Lerp(currentWobbleAmplitude, targetWobbleAmplitude, Time.deltaTime * 1f);
        currentWobbleFrequency = Vector3.Lerp(currentWobbleFrequency, targetWobbleFrequency, Time.deltaTime * 1f);

        Vector3 wobbleOffset = new Vector3(Mathf.Sin(currentWobblePhase.x) * currentWobbleAmplitude.x,
                                           Mathf.Sin(currentWobblePhase.y) * currentWobbleAmplitude.y,
                                           Mathf.Sin(currentWobblePhase.z) * currentWobbleAmplitude.z);

        transform.position = currentPosition + wobbleOffset;

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
        targetWobbleAmplitude = new Vector3(0.05f, 0.3f, 0.05f);
        targetWobbleFrequency = new Vector3(0.8f, 5f, 0.8f);
    }

    private void UpdateLeaving()
    {
        targetWobbleAmplitude = new Vector3(0,0.1f,0);
        targetWobbleFrequency = new Vector3(0,2,0);

        Vector3 pos = transform.position;
        float dif = Mathf.Abs(targetPosition.x - (leavingPoint.position.x));
        if (dif > 0.1f + randomXOffset) targetPosition.x -= Mathf.Sign(targetPosition.x - leavingPoint.position.x) * Time.deltaTime * 2;
        else targetPosition.z += Time.deltaTime * 2;

    }
}

