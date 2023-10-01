/*************************************************************************
// File Name:       GameManager.cs
// Author:          Andrew Stapay
// Creation Date:   September 18, 2023
//
// Description:     The Game Manager for Project Warp. Controls the camera
                    and sets the frame rate and resolution for the game.
*************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // The GameObject of the main character. Used to keep track of where the camera should be.
    [SerializeField] private GameObject pw350;

    // A frame counter used to keep track of how many frames to stop time
    private int freezeCounter;

    // <summary>
    // Start is called before the first frame update
    // Sets the resolution and framerate for the game
    // </summary>
    void Start()
    {
        //Setting resolution and frame rate
        Screen.SetResolution(1920, 1280, true);
        Application.targetFrameRate = 30;
    }

    // <summary>
    // Update is called once per frame
    // This will be used to have our camera follow the player
    // and to keep track of frozen time
    // </summary>
    void Update()
    {
        // We need access to the main camera to move it
        Camera c = Camera.main;

        // We will have it follow PW-350 as long as he is within certain bounds
        if (pw350.transform.position.x >= 0 && pw350.transform.position.x <= 42)
        {
            c.transform.position = new Vector3(pw350.transform.position.x, 0, -10);
        }

        // Decrement our freeze counter
        freezeCounter--;

        // If we are done with freeze frames, then we unfreeze the game
        if(freezeCounter == 0)
        {
            UnfreezeTime();
        }
    }

    public void FreezeTime(int frames)
    {
        Time.timeScale = 0;
        freezeCounter = frames;
    }

    private void UnfreezeTime()
    {
        Time.timeScale = 1;
    }

    public GameObject GetPW()
    {
        return pw350;
    }

    public float FindDistance(GameObject go1, GameObject go2)
    {
        float x1 = go1.transform.position.x;
        float x2 = go2.transform.position.x;
        float y1 = go1.transform.position.y;
        float y2 = go2.transform.position.y;

        float temp = (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
        return Mathf.Sqrt(temp);
    }
}
