using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    public MeshRenderer objectRenderer;
    public ParticleSystem particles;

    void Start()
    {
        
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0)){
            Debug.Log("Hit");
            objectRenderer.enabled = false;
            particles.Play();
            GetComponent<SphereCollider>().enabled = false; 
            Invoke("DestroyObject",2);

        }
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }
}
