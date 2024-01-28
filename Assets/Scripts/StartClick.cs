using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartClick : MonoBehaviour
{
    public GameObject start;

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            start.SetActive(true);
            Destroy(gameObject);
        }
    }
}
