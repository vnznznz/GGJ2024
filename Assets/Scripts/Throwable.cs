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

    private void Update()
    {
        if(Vector3.Distance(transform.position, GameObject.Find("Comedian").transform.position) < 0.5f)
        {

        }
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0)){
            Debug.Log("Hit");
            objectRenderer.enabled = false;
            particles.Play();
            GetComponent<SphereCollider>().enabled = false; 
            Invoke("DestroyObject",2);

            int index = Mathf.Clamp(Mathf.RoundToInt(UnityEngine.Random.value * 4), 0, 3);
            FMODUnity.RuntimeManager.PlayOneShot("event:/Impacts/Splash_"+(index+1).ToString(), Camera.main.transform.position + UnityEngine.Random.insideUnitSphere * 3);

        }
    }

    private void Explode()
    {
        Debug.Log("Hit");
        objectRenderer.enabled = false;
        particles.Play();
        GetComponent<SphereCollider>().enabled = false;
        Invoke("DestroyObject", 2);

        int index = Mathf.Clamp(Mathf.RoundToInt(UnityEngine.Random.value * 4), 0, 3);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Impacts/Splash_" + (index + 1).ToString(), Camera.main.transform.position + UnityEngine.Random.insideUnitSphere * 3);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Comedian") { Explode(); }
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }
}
