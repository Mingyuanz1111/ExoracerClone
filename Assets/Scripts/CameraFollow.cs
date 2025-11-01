using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour
{
    public bool isFollow = true;
    //private GameObject[] playerObjects;
    private GameObject playerObject;
    private GameObject baseObject;
    private Vector3 currentlyFollow;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    private void Start()
    {
        /*playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject eachPlayer in players)
        {
            if (eachPlayer.name == "Base")
            {
                baseObject = eachPlayer;
            }
            else
            {
                playerObject = eachPlayer;
            }
        }*/
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        /*if (playerObject.activeSelf && !gamemode.gameOver)
        {
            currentlyFollow = playerObject.transform;
        }
        else
        {
            currentlyFollow = baseObject.transform;
        }*/
        if (playerObject != null) currentlyFollow = playerObject.transform.position;
        if (isFollow)
        {
            Vector3 desiredPosition = currentlyFollow + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            ScreenCapture.CaptureScreenshot("screenshot_" + SceneManager.GetActiveScene().name + ".png");
        }
    }
}