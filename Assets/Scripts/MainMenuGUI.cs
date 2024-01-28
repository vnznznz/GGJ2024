using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class MainMenuGUI : MonoBehaviour
{
    private PostProcessVolume cam;
    private DepthOfField dof;

    private GameObject camMenu;
    private GameObject camTut1;
    private GameObject camTut2;

    private Cinemachine.CinemachineBrain brain;

    public GameObject menuCanvas;
    public GameObject tutorialCanvas;

    public List<GameObject> tutorialPages;


    private void Awake()
    {
        camMenu = GameObject.Find("CamPosMenu");
        camTut1 = GameObject.Find("CamPosTut1");
        camTut2 = GameObject.Find("CamPosTut2");

        cam = Camera.main.GetComponent<PostProcessVolume>();
        cam.profile.TryGetSettings(out dof);
        dof.focusDistance.value = 1.16f;

        brain = Camera.main.GetComponent<Cinemachine.CinemachineBrain>();   

        camTut1.SetActive(false);
        camTut2.SetActive(false);

        tutorialCanvas.SetActive(false);
        ResetTutorialPages();
    }

    public void StartRound()
    {
        GameManager.Instance.PrepareNewRound();
        dof.enabled.value = false;
    }

    public void StartTutorial()
    {
        menuCanvas.SetActive(false);
        camTut1.SetActive(true);
        camMenu.SetActive(false);

        GameManager.Instance.SpawnTutorialPeople();

        tutorialCanvas.SetActive(true);

        Invoke("ContinueTutorial",5);
    }

    void ContinueTutorial()
    {
        OpenNextTutorialPage();
        brain.m_DefaultBlend.m_Time = 20;
        camTut1.SetActive(false);
        camTut2.SetActive(true);

        Invoke("EndTutorial",22);
    }

    void EndTutorial()
    {
        brain.m_DefaultBlend.m_Time = 2;
        camTut2.SetActive(false);
        camMenu.SetActive(true);
        Invoke("StartMenu",2);
        Invoke("RemovePeople", 1f);
    }

    void StartMenu()
    {
        tutorialCanvas.SetActive(false);
        menuCanvas.SetActive(true);
    }

    void RemovePeople()
    {
        GameManager.Instance.RemoveTutorialPeople();
    }

    private int currentTutorialPage = 0;

    private void ResetTutorialPages()
    {
        foreach(var page in tutorialPages)
        {
            page.SetActive(false);
        }
    }

    private void OpenNextTutorialPage()
    {
        if (currentTutorialPage >= tutorialPages.Count)
        {
            Invoke("ResetTutorialPages", 1);
            return;
        }
        ResetTutorialPages();
        tutorialPages[currentTutorialPage].SetActive(true);
        currentTutorialPage++;

        Invoke("OpenNextTutorialPage",8);
    }


    
}
