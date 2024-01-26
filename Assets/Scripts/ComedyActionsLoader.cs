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

    public ComedyActionResult[] result;
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

    public ComedyAction[] GetRandomComedyActions(uint count = 1)
    {
        // selection sampling https://stackoverflow.com/a/48089
        ComedyAction[] choices = new ComedyAction[count];
        uint choiceIdx = 0;
        for (int i = 0; i < this.comedyActions.Length; i++)
        {
            var possibleChoicesCount = (float)this.comedyActions.Length - i;
            var choicesNeeded = (float)count - choiceIdx;
            var chance = choicesNeeded / possibleChoicesCount;

            if (UnityEngine.Random.Range(0f, 1f) <= chance)
            {
                choices[choiceIdx] = this.comedyActions[i];
                choiceIdx += 1;
            }

            if (choiceIdx == count)
                break;
        }
        if (choiceIdx < count)
        { Debug.LogError("Failed to gather enough comedy actions"); }
        return choices;
    }
}
