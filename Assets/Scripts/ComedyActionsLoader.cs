using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ComedyActionResult
{
    public string audience;
    public int value;
}


[System.Serializable]
public class ComedyAction
{
    public string title;
    public string text;

    public ComedyActionResult[] results;
}

[System.Serializable]
public class ComedyActionsList
{
    public ComedyAction[] actions;

}

public class ComedyActionsLoader : MonoBehaviour
{
    // Start is called before the first frame update
    public TextAsset actionsFile;
    public ComedyAction[] comedyActions;


    // Start is called before the first frame update
    void Start()
    {
        this.comedyActions = JsonUtility.FromJson<ComedyActionsList>(this.actionsFile.text).actions;

        //foreach (var item in this.comedyActions)
        //{
        //    Debug.Log(item.text);
       // }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
