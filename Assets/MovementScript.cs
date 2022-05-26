using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using UnityEngine.EventSystems;

public class MovementScript : MonoBehaviour
{
    public GameObject placedPrefab;
    public Camera arCamera;
    public AudioSource audioSource;
    private GameObject placedObject;
    private Vector2 touchPosition = default;
    private ARRaycastManager arRaycastManager;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private int activeBlips = 1;

    // Start is called before the first frame update
    void Start()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // If touch detected
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            touchPosition = touch.position;

            // If first touch detected and the pointer isnt over the UI (Prevent tapping through UI)
            if(touch.phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                // Create ray from camera into world space
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;

                // If ray hits a blip, delete it
                if(Physics.Raycast(ray, out hitObject))
                {
                    Destroy(hitObject.transform.gameObject);
                    activeBlips -= 1;
                }

                // Otherwise, create a new blip 0.5m away from camera with appropriate frequency
                else
                {        
                    activeBlips += 1;
                    audioSource.pitch = ray.GetPoint(0.5f).y + 1.0f;
                    Instantiate(placedPrefab, ray.GetPoint(0.5f), Quaternion.identity);
                }
            }

            // Code for exponentially decreasing vol of all blips to avoid aliasing (1/N)
            GameObject[] spheres = GameObject.FindGameObjectsWithTag("Untagged");
            for(int i = 0; i < spheres.Length; i++)
            {
                spheres[i].GetComponent<AudioSource>().volume = (1 / activeBlips);
            }
        }
    }
}
