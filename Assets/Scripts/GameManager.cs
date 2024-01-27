using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;

    public GameObject PersonPrefab;

    public ComedyActionsLoader comedyActionsLoader;

    private List<Person> audience = new List<Person>();

    int index = 0;


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

        }
    }

    void Start()
    {
        PopulateAudience();
        comedyActionsLoader = GetComponent<ComedyActionsLoader>();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TellAJoke(comedyActionsLoader.comedyActions[UnityEngine.Random.Range(0, comedyActionsLoader.comedyActions.Length)]);
        }
    }


    public void PopulateAudience()
    {
        GameObject[] chairs = GameObject.FindGameObjectsWithTag("Chair");

        string[] ageTags = { "boomer", "millenial", "genz" };
        string[] genderTags = { "male", "female" };
        foreach (GameObject chair in chairs)
        {
            GameObject newPerson = Instantiate(PersonPrefab, chair.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
            var person = newPerson.GetComponent<Person>();
            person.audienceTags = new string[2];
            person.audienceTags[0] = ageTags[UnityEngine.Random.Range(0, ageTags.Length)];
            person.audienceTags[1] = genderTags[UnityEngine.Random.Range(0, genderTags.Length)];
            audience.Add(newPerson.GetComponent<Person>());
        }
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
            item.enjoymentValue += enjoymentValues[item];
            item.SetBehavior();
            Debug.Log($"{item.audienceTags[0]}, {item.audienceTags[1]}: {enjoymentValues[item]}");
        }

    }

    void PersonLeave()
    {
        audience[index].behaviorState = Person.BehaviorState.Leaving;
        index++;
    }
}
