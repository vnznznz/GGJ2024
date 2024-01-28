using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Titlecard : MonoBehaviour
{

    public bool flyingIn = true;

    private float scale = 0;
    private float fadeOut = 1;

    public GameObject flyingText;
    private TextMeshProUGUI text;

    private bool newTextFlying = false;
    private bool alreadyTitle = false;

    public bool flyingIn2 = true;

    private float scale2=0;
    private float fadeIn2 = 0;

    public GameObject flyingTitle;
    private TextMeshProUGUI title;

    public Image startButton;

    public AudioSource startAudio;

    void OnEnable()
    {
        text = flyingText.GetComponent<TextMeshProUGUI>();
        title = flyingTitle.GetComponent<TextMeshProUGUI>();
        title.transform.localScale = Vector3.zero;
        var tempColor = startButton.color;
        tempColor.a = fadeIn2;
        startButton.color = tempColor;

        Invoke("StartAudio",0.15f);
    }

    private void Update()
    {
        if (flyingIn) scale += Time.deltaTime * 0.2f;
        if(scale>=1) flyingIn = false;
        if (!flyingIn)
        {
            if (fadeOut > 0) fadeOut -= Time.deltaTime * 0.5f;
            if (!alreadyTitle)
            {
                Invoke("AllowTitleFly", 3);
                alreadyTitle = true;
            }
        }

        flyingText.transform.localScale = new Vector3(1,1,1) * scale;
        var tempColor = text.color;
        tempColor.a = fadeOut;
        text.color = tempColor;



        if (newTextFlying)
        {
            if (flyingIn2) scale2 += Time.deltaTime * 5;
            if (scale2 >= 1) flyingIn2 = false;
            if (!flyingIn2)
            {
                if (fadeIn2 < 1) fadeIn2 += Time.deltaTime * 0.5f;
            }

            
        }

        flyingTitle.transform.localScale = new Vector3(1, 1, 1) * scale2;
        var tempColor2 = startButton.color;
        tempColor2.a = fadeIn2;
        startButton.color = tempColor2;
    }

    void AllowTitleFly()
    {
        newTextFlying = true;
    }

    void StartAudio()
    {
        startAudio.Play();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

}
