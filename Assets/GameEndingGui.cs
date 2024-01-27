using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameEndingGui : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateText(string gamestateText, string audienceStats)
    {
        transform.Find("GameStateText").GetComponent<TextMeshProUGUI>().text = gamestateText;
        transform.Find("AudienceStats").GetComponent<TextMeshProUGUI>().text = audienceStats;

    }
    public void RestartGame()
    {
        GameManager.Instance.RestartGame();
    }
}
