using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class WrapCamera : MonoBehaviour
{
    private static Action OnCamSpawned;
    [SerializeField] float upperBound;
    [SerializeField] float lowerBound;
    [SerializeField] Transform target;
    [SerializeField] TextMeshProUGUI playerText;
    [SerializeField] public GameObject OverlayCamera { private get; set; }

    private Camera followCam;
    
    float xDeadZone = 5;
    float yDeadZone = 3;

    private float velocityX = 0.0f;
    private float velocityY = 0.0f;
    private Vector3 velocity = Vector3.zero;
    private float smoothTime = 2f; // Time taken to reach the target. Adjust this value as needed.
    private float smoothTimeX = 1f;
    private float smoothTimeY = 1f;


    private float zpos = -10;
    private Vector2 delta;

    CameraCorners cameraCorners;

    private float minCamYPos;

    private static int camCount = 0;

    private int camId;

    private void Awake()
    {
        OnCamSpawned += SetViewPort;
    }

    private void Start()
    {
       
        Debug.Log($"Zpos {zpos}");
        followCam = gameObject.GetComponent<Camera>();
        
        cameraCorners = CameraExtensions.GetViewportCorners(followCam, zpos);
        minCamYPos = Vector2.Distance(cameraCorners.TopRight, cameraCorners.BottomRight) * .5f;
        camCount++;
        camId = camCount;
        playerText.text = camId.ToString();
        OnCamSpawned?.Invoke();
    }


    private void SetViewPort()
    {
        // Change the view port according to these conditions
        if (camCount == 1)
        {
            // Full Screen;
            followCam.rect = new Rect(0f, 0f, 1f, 1f);
            OverlayCamera.GetComponent<Camera>().rect = new Rect(0f, 0f, 1f, 1f);
        }
        else if (camId == 1)
        {
            // Top half of screen
            followCam.rect = new Rect(0f, 0.5f, 1f, 0.5f);
            OverlayCamera.GetComponent<Camera>().rect = new Rect(0f, 0.5f, 1f, 0.5f);
        }
        else if (camId == 2)
        {
            // Bottom half of screen
            followCam.rect = new Rect(0f, 0f, 1f, 0.49f);
            OverlayCamera.GetComponent<Camera>().rect = new Rect(0f, 0f, 1f, 0.49f);
        }
    }




    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void SetTargetAndLock(Transform target)
    {
        this.target = target;
        Vector3 targetPos = target.position;
        transform.position = new Vector3(targetPos.x + delta.x, targetPos.y + delta.y, zpos);
    }

    private void LateUpdate()
    {
        if (target == null) return;
        Vector3 targetPos = target.position;
        float dist = Mathf.Abs(transform.position.x - targetPos.x); //(new Vector2(transform.position.x, transform.position.y) - new Vector2(targetPos.x, targetPos.y)).magnitude;
        if(dist > 30)
        {
            transform.position = new Vector3(targetPos.x + delta.x, targetPos.y + delta.y, zpos);
        }
        else
        {
            Vector3 followPos = transform.position;
         
            if (Mathf.Abs(targetPos.x-followPos.x) > xDeadZone)
            {
                followPos.x = Mathf.SmoothDamp(followPos.x, targetPos.x, ref velocityX, smoothTimeX);
            }
            

            if (Mathf.Abs(targetPos.y - followPos.y) > yDeadZone)
            {
                followPos.y = Mathf.SmoothDamp(followPos.y, targetPos.y, ref velocityY, smoothTimeY);
            }
            

            followPos.y = Mathf.Clamp(followPos.y, minCamYPos, 100);

            transform.position = followPos;

            delta.x = transform.position.x - target.position.x;
            delta.y = transform.position.y - target.position.y;
          
        }

        CameraEdges edges = CameraExtensions.GetViewportEdges(followCam, 10);
        if(edges.LeftEdge <= WrapWorld.leftBound)
        {
            float delta = transform.position.x - WrapWorld.leftBound;
            Vector3 pos = transform.position;
            pos.x = WrapWorld.rightBound + delta;
            OverlayCamera.transform.position = pos;
            OverlayCamera.SetActive(true);
        }
        else if(edges.RightEdge >= WrapWorld.rightBound)
        {
            float delta = transform.position.x - WrapWorld.rightBound;
            Vector3 pos = transform.position;
            pos.x = WrapWorld.leftBound + delta;
            OverlayCamera.transform.position = pos;
            OverlayCamera.SetActive(true);
        }
        else
        {
            OverlayCamera.SetActive(false);
        }
        
    }

    private void OnDestroy()
    {
        OnCamSpawned -= SetViewPort;
    }

}
