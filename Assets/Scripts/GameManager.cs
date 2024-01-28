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

    private GameObject menuCamera;
    private GameObject gameCamera;
    private GameObject mainMenu;

    private List<Person> audience = new List<Person>();

    public int numCards = 3;


    private ComedyAction[] currentCards;

    public Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<int, EventReference>>>> laughingSounds;

    public float gameDuration = 5f;
    public float currentGameTime = 0;
    private float startTime;

    public float jokeIntensityModifier = 1.5f;
    public float negativeIntensityModifier = 2f;

    [FMODUnity.BankRef]
    public List<string> Banks;

    public string Scene = "SampleScene";

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

    IEnumerator LoadGameAsync()
    {
        // Start an asynchronous operation to load the scene
        AsyncOperation async = SceneManager.LoadSceneAsync(Scene);

        // Don't lead the scene start until all Studio Banks have finished loading
        async.allowSceneActivation = false;

        // Iterate all the Studio Banks and start them loading in the background
        // including the audio sample data
        foreach (var bank in Banks)
        {
            FMODUnity.RuntimeManager.LoadBank(bank, true);
        }

        // Keep yielding the co-routine until all the Bank loading is done
        while (FMODUnity.RuntimeManager.AnyBankLoading())
        {
            yield return null;
        }

        // Allow the scene to be activated. This means that any OnActivated() or Start()
        // methods will be guaranteed that all FMOD Studio loading will be completed and
        // there will be no delay in starting events
        async.allowSceneActivation = true;

        // Keep yielding the co-routine until scene loading and activation is done.
        while (!async.isDone)
        {
            yield return null;
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
        gameState = GameState.Menu;
        currentGameTime = 0;
        comedyActionsLoader = GetComponent<ComedyActionsLoader>();

        cardManager = FindFirstObjectByType<CardManager>(FindObjectsInactive.Include);
        menuCamera = GameObject.Find("CamPosMenu");
        gameCamera = GameObject.Find("CamPosGame");
        mainMenu = GameObject.Find("Menu");

        cardManager.gameObject.SetActive(false);
        menuCamera.SetActive(true);
        gameCamera.SetActive(false);
        mainMenu.SetActive(true);

    }



    void Update()
    {

        switch (gameState)
        {
            case GameState.Menu:
                break;
            case GameState.Round:
                UpdateGame();
                break;
            case GameState.GameOver:
                break;
        }

    }


    private void UpdateGame()
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
        if (getAudienceSatisfaction() <= 0.01)
        {
            GameOver();
        }
    }


    public void GameOver()
    {
        gameState = GameState.GameOver;
        cardManager.gameObject.SetActive(false);
        var gameOverGui = FindFirstObjectByType<GameEndingGui>(FindObjectsInactive.Include);
        var satisfaction = getAudienceSatisfaction();
        var leaverCount = getDissatisfiedAudienceCountCalculationRoutine();
        var remainingAudience = audience.Count - leaverCount;

        if (satisfaction <= 0.01)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/GameOver/GameOver_very_pleased", transform.position);
            gameOverGui.UpdateText(
            "Failed Show!",
            $"{leaverCount} guests left the show. Maybe try to be funny next time?");
        }
        else if (satisfaction <= 0.2)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/GameOver/GameOver_pleased", transform.position);
            gameOverGui.UpdateText(
            "Horrible Show!",
            $"Only {remainingAudience} of {audience.Count} guests stayed until the end.");
        }
        else if (satisfaction <= 0.8)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/GameOver/GameOver_bad", transform.position);
            gameOverGui.UpdateText(
            "Good Show!",
            $"Only {leaverCount} of {audience.Count} guests left. The rest had a (mostly) good time.");
        }
        else
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/GameOver/GameOver_horrible", transform.position);
            gameOverGui.UpdateText(
            "Successful Show!",
            $"About {UnityEngine.Random.Range(1, 5)} people wet their pants laughing. Only {this.getDissatisfiedAudienceCountCalculationRoutine()} of {this.audience.Count} guests left. ");
        }

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

    private List<Person> tutorialPeople;

    public void SpawnTutorialPeople()
    {
        GameObject[] chairs = GameObject.FindGameObjectsWithTag("Chair");
        tutorialPeople = new List<Person>();

        string[] ageTags = { "boomer", "millenial", "genz" };
        string[] genderTags = { "male", "female" };
        int index = 0;
        foreach (GameObject chair in chairs)
        {
            if (chair.layer != 10) continue;
            GameObject newPerson = Instantiate(PersonPrefab, chair.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
            var person = newPerson.GetComponent<Person>();
            person.audienceTags = new string[2];
            person.audienceTags[1] = ageTags[(int)(index % 3)];
            person.audienceTags[0] = genderTags[(int)(index / 3f)];
            index++;
            tutorialPeople.Add(newPerson.GetComponent<Person>());
        }
    }

    public void RemoveTutorialPeople()
    {
        foreach (Person person in tutorialPeople)
        {
            Destroy(person.gameObject);
        }
        tutorialPeople.Clear();
    }

    public float getTimeLeft()
    {
        return gameDuration - currentGameTime;
    }
    public void MissJoke()
    {
        foreach (Person person in audience)
        {
            person.ForceEnjoymentUponThee(-15, true);
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
            var enj = enjoymentValues[item];

            if (enj < 0)
            {
                enj *= negativeIntensityModifier;
            }

            enj = enj * jokeIntensityModifier + UnityEngine.Random.Range(-5, 5);
            item.ForceEnjoymentUponThee(enj, false);
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

        // audience satisfaction goes from 0-1
        // it consists of two equally weighted parts
        // how many guests left until only half of the audience
        // total current enjoyment value of each guest left in the audience / total possibe enjoyment value

        // if audience satisfaction goes under 0.3 you lose

        float sumEnjoyment = 0f;

        var angryGuests = getDissatisfiedAudienceCountCalculationRoutine();
        var halfAudienceCount = audience.Count / 2;
        var leftOverAudience = audience.Count - angryGuests;
        var guestsLeftUntilHalf = Math.Clamp(halfAudienceCount - angryGuests, 0, halfAudienceCount);
        var guestsLeftScore = (guestsLeftUntilHalf / (float)halfAudienceCount);
        foreach (var item in audience)
        {
            if (item.enjoymentValue > 0f)
            { sumEnjoyment += item.getUnitEnjoymentVector1D(); }
        }

        var totalSatisfaction = (guestsLeftScore + (sumEnjoyment / leftOverAudience)) / 2f;
        //sumEnjoyment /= audience.Count / 2;
        // audienceSatisfaction = 0f - 1f
        // go to 0 when half of the audience has left

        var loseThreshold = 0.3f;

        return (totalSatisfaction - loseThreshold) / (1 - loseThreshold);

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
        gameCamera.SetActive(true);
        menuCamera.SetActive(false);
        mainMenu.SetActive(false);

        Invoke("StartNewRound", 2);
    }

    public void StartNewRound()
    {
        PopulateAudience();
        cardManager.gameObject.SetActive(true);
        currentCards = comedyActionsLoader.GetRandomComedyActions((uint)numCards);
        gameState = GameState.Round;

        startTime = Time.time;
        currentGameTime = 0;
    }


    private void ResetGameState()
    {
        currentGameTime = 0;
        gameState = GameState.Round;
    }


}
