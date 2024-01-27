using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuGUI : MonoBehaviour
{
    public void StartRound()
    {
        GameManager.Instance.PrepareNewRound();
    }
}
