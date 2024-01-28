using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroller : MonoBehaviour
{
    public MainMenuGUI mainMenu;

    private void OnDisable()
    {
        transform.position -= new Vector3(0, 3400, 0);
    }


    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0,Time.deltaTime*100,0);

        if (transform.position.y > 3400)
        {
            mainMenu.menuCanvas.SetActive(true);
            mainMenu.creditsCanvas.SetActive(false);
        }
    }
}
