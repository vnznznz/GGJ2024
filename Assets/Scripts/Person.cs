using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[Serializable]
public struct NamedImage
{
    public string name;
    public Sprite image;
}

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

    private MeshRenderer meshRenderer;

    public float enjoymentValue = 50;

    private Transform leavingPoint;

    public List<GameObject> throwableObjects;

    private FMODUnity.StudioEventEmitter emitter;

    public enum BehaviorState
    {
        Neutral,
        Happy,
        Booing,
        Angry,
        Leaving
    }

    public Sprite faceNeutral;
    public Sprite faceHappy;
    public Sprite faceBooing;
    public Sprite faceAngry;
    public Sprite faceLeaving;
    public BehaviorState behaviorState = BehaviorState.Neutral;

    public Camera mainCamera;
    public UnityEngine.UI.Image faceImage;

    public UnityEngine.UI.Image reactionImage;

    public string[] audienceTags;


    public NamedImage[] audienceHairImages;
    public NamedImage[] reactionIconImages;

    private Coroutine hideImageCoroutine;

    private Dictionary<string, Sprite> reactionImages = new Dictionary<string, Sprite>();
    void Start()
    {
        startPosition = transform.position;
        targetPosition = transform.position;
        currentPosition = transform.position;
        randomXOffset = UnityEngine.Random.Range(0, 0.5f);

        currentWobblePhase = new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);

        leavingPoint = GameObject.Find("LeavingPosition").transform;
        meshRenderer = GetComponent<MeshRenderer>();

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        faceImage = transform.Find("Canvas/FaceSprite").GetComponent<UnityEngine.UI.Image>();
        faceImage.sprite = faceNeutral;

        reactionImage = transform.Find("Reaction/ReactionSprite").GetComponent<UnityEngine.UI.Image>();

        emitter = GetComponent<FMODUnity.StudioEventEmitter>();

        var hairId = $"{gender}_{age}";

        foreach (var item in audienceHairImages)
        {
            if (item.name == hairId)
            {
                transform.Find("Canvas/HairSprite").GetComponent<UnityEngine.UI.Image>().sprite = item.image;
                break;
            }
        }

        foreach (var item in reactionIconImages)
        {
            reactionImages[item.name] = item.image;
        }
    }

    public string gender
    {
        get { return audienceTags[0]; }
    }

    public string age
    {
        get { return audienceTags[1]; }
    }

    void Update()
    {
        UpdateBehavior();
        UpdateMovement();

        float enj = 1f - (Math.Clamp(this.enjoymentValue, 1, 100) / 100f);
        var col = Color.Lerp(new Color(1, 0.859f, 0.06f), Color.red, enj);
        meshRenderer.material.SetColor("_Color", col);

        transform.LookAt(mainCamera.transform);

        enjoymentValue = Math.Clamp(enjoymentValue, -1, 100);
    }

    private IEnumerator HideReactionIcon(float seconds)
    {

        float alpha = 1;
        reactionImage.color = new Color(1f, 1f, 1f, 1f);

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / seconds)
        {
            Color newColor = new Color(1f, 1f, 1f, Mathf.Lerp(alpha, 0, t));
            reactionImage.color = newColor;
            yield return null;
        }

        reactionImage.sprite = null;

        yield return null;
    }

    public void ForceEnjoymentUponThee(float enjoymentDelta, bool silence)

    {
        if (enjoymentValue < 0)
        {
            ThrowAtComedian();
            return;
        }
        enjoymentValue += enjoymentDelta;

        if (enjoymentDelta >= 10)
        {
            // happy icon
            reactionImage.sprite = reactionImages["happy"];
        }
        else if (enjoymentDelta > 5 && enjoymentDelta < 10)
        {
            // mild happy icon
            reactionImage.sprite = reactionImages["mildhappy"];
        }
        else if (enjoymentDelta < -5 && enjoymentDelta > -10)
        {
            // mild unhappy icon
            reactionImage.sprite = reactionImages["mildunhappy"];
        }
        else if (enjoymentDelta <= -10)
        {
            // mild happy icon
            reactionImage.sprite = reactionImages["unhappy"];
        }
        else
        {
            reactionImage.sprite = reactionImages["question"];
        }



        if (hideImageCoroutine != null)
        {
            StopCoroutine(hideImageCoroutine);
        }
        hideImageCoroutine = StartCoroutine(HideReactionIcon(3));

        if (enjoymentValue >= 70) behaviorState = BehaviorState.Happy;
        else if (enjoymentValue >= 40) behaviorState = BehaviorState.Neutral;
        else if (enjoymentValue >= 20) behaviorState = BehaviorState.Booing;
        else if (enjoymentValue >= 0.1) behaviorState = BehaviorState.Angry;
        else behaviorState = BehaviorState.Leaving;

        if (enjoymentValue > 10 && enjoymentValue < 40)
        {
            ThrowAtComedian();
        }

        if (!silence) React();

        if (behaviorState == BehaviorState.Leaving)
        {
            reactionImage.sprite = reactionImages["ded"];
            reactionImage.color = Color.white;
            StopCoroutine(hideImageCoroutine);
        }
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
        faceImage.sprite = faceNeutral;
    }
    private void UpdateHappy()
    {
        targetWobbleAmplitude = new Vector3(0.01f, 0.2f, 0.01f);
        targetWobbleFrequency = new Vector3(0.5f, 0.8f, 0.5f);
        faceImage.sprite = faceHappy;
    }

    private void UpdateBooing()
    {
        targetWobbleAmplitude = new Vector3(0.05f, 0.2f, 0.05f);
        targetWobbleFrequency = new Vector3(0.8f, 1f, 0.8f);
        faceImage.sprite = faceBooing;
    }

    private void UpdateAngry()
    {
        targetWobbleAmplitude = new Vector3(0.05f, 0.3f, 0.05f);
        targetWobbleFrequency = new Vector3(0.8f, 5f, 0.8f);
        faceImage.sprite = faceAngry;
    }

    private void UpdateLeaving()
    {
        targetWobbleAmplitude = new Vector3(0, 0.1f, 0);
        targetWobbleFrequency = new Vector3(0, 2, 0);
        faceImage.sprite = faceLeaving;
        Vector3 pos = transform.position;
        float dif = Mathf.Abs(targetPosition.x - (leavingPoint.position.x));
        if (dif > 0.1f + randomXOffset) targetPosition.x -= Mathf.Sign(targetPosition.x - leavingPoint.position.x) * Time.deltaTime * 2;
        else targetPosition.z += Time.deltaTime * 2;

    }

    public void ThrowAtComedian()
    {
        int numThrows = (int)UnityEngine.Random.Range(1, 3);

        for (int i = 0; i < numThrows; i++)
        {
            Invoke("ThrowObject", i * 0.5f + UnityEngine.Random.Range(0, 0.3f));
        }

    }

    private void ThrowObject()
    {
        int randIndex = Mathf.RoundToInt(UnityEngine.Random.value * (throwableObjects.Count - 1));
        GameObject throwObj = Instantiate(throwableObjects[randIndex], transform.position, Quaternion.identity);

        Vector3 dirToComedian = (GameObject.Find("Comedian").transform.position - transform.position).normalized;

        throwObj.GetComponent<Rigidbody>().AddForce(dirToComedian * 100 + Vector3.up * 70);
        throwObj.GetComponent<Rigidbody>().AddTorque(UnityEngine.Random.insideUnitSphere);

    }

    public void React()
    {
        switch (behaviorState)
        {
            case BehaviorState.Neutral:
                PlayAwkwardLaugh();
                break;
            case BehaviorState.Happy:
                PlayHappyLaugh();
                break;
            case BehaviorState.Booing:
                PlayBooing();
                break;
            case BehaviorState.Angry:
                PlayAngry();
                break;
            case BehaviorState.Leaving:
                PlayLeaving();
                break;

        }
    }

    private void PlayAwkwardLaugh()
    {
        int laughIndex = Mathf.Clamp(Mathf.RoundToInt(UnityEngine.Random.value * 4), 0, 3);

        string path = "event:/Audience/AwkwardLaughing" + "/" + audienceTags[0] + "/" + audienceTags[0] + "_" + audienceTags[1] + "_awkward_" + (laughIndex + 1).ToString();
        FMODUnity.RuntimeManager.PlayOneShot(path, transform.position);
    }

    private void PlayHappyLaugh()
    {
        int laughIndex = Mathf.Clamp(Mathf.RoundToInt(UnityEngine.Random.value * 4), 0, 3);

        string path = "event:/Audience/Laughing" + "/" + audienceTags[0] + "/" + audienceTags[0] + "_" + audienceTags[1] + "_laugh_" + (laughIndex + 1).ToString();
        FMODUnity.RuntimeManager.PlayOneShot(path, transform.position);
    }

    private void PlayBooing()
    {
        int laughIndex = Mathf.Clamp(Mathf.RoundToInt(UnityEngine.Random.value * 2), 0, 1);

        string path = "event:/Audience/Booing" + "/" + audienceTags[0] + "/" + audienceTags[0] + "_" + audienceTags[1] + "_booing_" + (laughIndex + 1).ToString();
        FMODUnity.RuntimeManager.PlayOneShot(path, transform.position);
    }

    private void PlayAngry()
    {
        int laughIndex = Mathf.Clamp(Mathf.RoundToInt(UnityEngine.Random.value * 4), 0, 3);

        string path = "event:/Audience/Angry" + "/" + audienceTags[0] + "/" + audienceTags[0] + "_" + audienceTags[1] + "_angry_" + (laughIndex + 1).ToString();
        FMODUnity.RuntimeManager.PlayOneShot(path, transform.position);
    }

    private void PlayLeaving()
    {
        int index = Mathf.Clamp(Mathf.RoundToInt(UnityEngine.Random.value * 7), 0, 6);

        string path = "event:/Audience/Leaving/ChairMoving" + (index + 1).ToString();
        FMODUnity.RuntimeManager.PlayOneShot("event:/Audience/Leaving/ChairMoving_" + (index + 1).ToString(), transform.position);
        index = Mathf.Clamp(Mathf.RoundToInt(UnityEngine.Random.value * 4), 0, 3);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Audience/Leaving/Footsteps_" + (index + 1).ToString(), transform.position);
    }

    public void Despawn()
    {
        CancelInvoke("ThrowObject");
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Despawn")
        {
            Despawn();
        }
    }

}
