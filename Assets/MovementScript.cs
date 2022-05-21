using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;

public class MovementScript : MonoBehaviour
{
    public GameObject placedPrefab;
    public Camera arCamera;
    public AudioSource audioSource;
    private GameObject placedObject;
    private Vector2 touchPosition = default;
    private ARRaycastManager arRaycastManager;
    private bool onTouchHold = false;
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
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            touchPosition = touch.position;

            if(touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;
                if(Physics.Raycast(ray, out hitObject))
                {

                    Destroy(hitObject.transform.gameObject);
                    activeBlips -= 1;
                }
                else
                {        
                    activeBlips += 1;
                    audioSource.pitch = ray.GetPoint(0.5f).y + 1.0f;
                    Instantiate(placedPrefab, ray.GetPoint(0.5f), Quaternion.identity);
                }
            }
            GameObject[] spheres = GameObject.FindGameObjectsWithTag("Untagged");
            for(int i = 0; i < spheres.Length; i++)
            {
                spheres[i].GetComponent<AudioSource>().volume = (1 / activeBlips);
            }
        }
        // if(arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.All))
        // {
        //     Pose hitPose = hits[0].pose;



        //     if(onTouchHold)
        //     {
        //         placedObject.transform.position = hitPose.position;
        //     }
        // }
    }
}
