using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;

    public GameObject PersonPrefab;


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
        Invoke("TellAJoke",3);
    }


    void Update()
    {
        
    }


    public void PopulateAudience()
    {
        GameObject[] chairs = GameObject.FindGameObjectsWithTag("Chair");

        foreach (GameObject chair in chairs)
        {
            GameObject newPerson = Instantiate(PersonPrefab,chair.transform.position + new Vector3(0,0.5f,0), Quaternion.identity);
            audience.Add(newPerson.GetComponent<Person>());
        }
    }

    public void TellAJoke()
    {
        
        foreach(Person person in audience)
        {
            // calculate each persons enjoyment

            // Update persons behavior

            // Update Score
            Invoke("PersonLeave",Random.Range(0, 1f));
        }

    }

    void PersonLeave()
    {
        audience[index].behaviorState = Person.BehaviorState.Leaving;
        index++;
    }
}
