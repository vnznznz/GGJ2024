using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;

    public GameObject PersonPrefab;

    public ComedyActionsLoader comedyActionsLoader;

    public CardManager cardManager;

    public GameObject menuCamera;
    public GameObject gameCamera;

    private List<Person> audience = new List<Person>();

    public int numCards = 3;


    private ComedyAction[] currentCards;

    public Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<int, EventReference>>>> laughingSounds;

    public float gameDuration = 5f;
    public float currentGameTime = 0;
    private float startTime;

    public enum GameState
    {
        Menu,
        Round,
        GameOver
    }

    public GameState gameState = GameState.Round;
    public int roundID = 0;



    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);



        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            SceneManager.activeSceneChanged -= ChangedActiveScene;
            SceneManager.activeSceneChanged += ChangedActiveScene;
        }

    }


    private void ChangedActiveScene(Scene current, Scene next)
    {
        string currentName = current.name;

        if (currentName == null)
        {
            // Scene1 has been removed
            currentName = "Replaced";
        }

        Debug.Log("Scenes: " + currentName + ", " + next.name);

        StartGame();
    }

    void StartGame()
    {
        gameState = GameState.Round;
        PopulateAudience();
        comedyActionsLoader = GetComponent<ComedyActionsLoader>();
        currentGameTime = 0;
        cardManager.gameObject.SetActive(false);
        menuCamera.SetActive(true);
        gameCamera.SetActive(false);
    }


    void Update()
    {


        currentGameTime += Time.deltaTime;

        if (getTimeLeft() <= 0)
        {
            GameOver();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            GameOver();
            //TellAJoke(comedyActionsLoader.comedyActions[UnityEngine.Random.Range(0, comedyActionsLoader.comedyActions.Length)]);
        }
    }

    public void GameOver()
    {
        gameState = GameState.GameOver;
        FindFirstObjectByType<CardManager>().gameObject.SetActive(false);
        var gameOverGui = FindFirstObjectByType<GameEndingGui>(FindObjectsInactive.Include);
        gameOverGui.UpdateText(
            "Successful Show!",
            $"Only {this.getDissatisfiedAudienceCountCalculationRoutine()} of {this.audience.Count} guests left the show.");
        gameOverGui.gameObject.SetActive(true);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        gameState = GameState.GameOver;

    }

    // ========== CUSTOM FUNCTIONS ==========

    public void PopulateAudience()
    {
        GameObject[] chairs = GameObject.FindGameObjectsWithTag("Chair");
        audience = new List<Person>();

        string[] ageTags = { "boomer", "millenial", "genz" };
        string[] genderTags = { "male", "female" };
        foreach (GameObject chair in chairs)
        {
            GameObject newPerson = Instantiate(PersonPrefab, chair.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
            var person = newPerson.GetComponent<Person>();
            person.audienceTags = new string[2];
            person.audienceTags[1] = ageTags[UnityEngine.Random.Range(0, ageTags.Length)];
            person.audienceTags[0] = genderTags[UnityEngine.Random.Range(0, genderTags.Length)];
            audience.Add(newPerson.GetComponent<Person>());
        }
    }

    public float getTimeLeft()
    {
        return gameDuration - currentGameTime;
    }
    public void MissJoke()
    {
        foreach (Person person in audience)
        {
            person.ForceEnjoymentUponThee(-5, true);
        }

        PlayAwkwardSilence();
    }

    public void TellAJoke(ComedyAction joke)
    {
        // calculate each persons enjoyment
        Dictionary<Person, float> enjoymentValues = new Dictionary<Person, float>();
        foreach (Person person in audience)
        {
            foreach (ComedyActionResult result in joke.result)
            {
                foreach (string audienceTag in person.audienceTags)
                {
                    if (result.audience == audienceTag)
                    {
                        if (enjoymentValues.ContainsKey(person))
                        {
                            enjoymentValues[person] += result.value;
                        }
                        else
                        {
                            enjoymentValues[person] = result.value;
                        }
                    }
                }
            }
            Debug.Log(joke.text);

        }

        foreach (var item in enjoymentValues.Keys)
        {
            // TODO:  notify person about enjoyment value change so it can display an emoji
            item.ForceEnjoymentUponThee(enjoymentValues[item], false);
            Debug.Log($"{item.audienceTags[0]}, {item.audienceTags[1]}: {enjoymentValues[item]}");
        }

    }

    public int getDissatisfiedAudienceCountCalculationRoutine()
    {
        int sum = 0;
        foreach (var item in audience)
        {
            if (item.enjoymentValue <= 0f)
            { sum += 1; }

        }

        return sum;
    }
    public float getAudienceSatisfaction()
    {
        float sumEnjoyment = 0f;

        foreach (var item in audience)
        {
            if (item.enjoymentValue > 0f)
            { sumEnjoyment += 1f; }
        }

        sumEnjoyment /= audience.Count;

        return sumEnjoyment;

    }

    public void StartRound()
    {
        gameState = GameState.Round;

        // Generate random set of cards
        currentCards = comedyActionsLoader.GetRandomComedyActions((uint)numCards);

        // update ui

    }

    public void SelectCard(int index)
    {
        TellAJoke(currentCards[index]);
    }

    public void PlayAwkwardSilence()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Audience/Silence/crickets", transform.position);

        int numCoughs = (int)UnityEngine.Random.Range(0, 4);

        for (int i = 0; i < numCoughs; i++)
        {
            Invoke("PlayCough", i * 0.6f + UnityEngine.Random.value * 0.5f);
        }


    }

    private void PlayCough()
    {
        int index = Mathf.Clamp(Mathf.RoundToInt(UnityEngine.Random.value * 7), 0, 6);
        string path = "event:/Audience/Silence/Coughing/Couch_" + (index + 1).ToString();
        FMODUnity.RuntimeManager.PlayOneShot(path, Camera.main.transform.position + UnityEngine.Random.insideUnitSphere * 3);
    }
    public void PrepareNewRound()
    {
        ResetRound();
        gameCamera.SetActive(true);
        menuCamera.SetActive(false);

        Invoke("StartNewRound",2);
    }

    public void StartNewRound()
    {
        PopulateAudience();
        cardManager.gameObject.SetActive(true);
        startTime = Time.time;
        currentGameTime = 0;
    }

    private void ResetRound()
    {
        Throwable[] thrownStuff = GameObject.FindObjectsOfType<Throwable>();
        foreach (Throwable throwable in thrownStuff)
        {
            Destroy(throwable.gameObject);
        }

        foreach(Person person in audience)
        {
            Destroy(person);
        }
        audience.Clear();

    }

    private void ResetGameState()
    {
        currentGameTime = 0;
        gameState = GameState.Round;
    }


}
