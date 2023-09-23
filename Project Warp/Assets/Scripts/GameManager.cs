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

    // <summary>
    // Start is called before the first frame update
    // </summary>
    void Start()
    {
        //Setting resolution and frame rate
        Screen.SetResolution(1920, 1280, true);
        Application.targetFrameRate = 30;
    }

    // <summary>
    // Update is called once per frame
    // </summary>
    void Update()
    {
        //We need access to the main camera to move it
        Camera c = Camera.main;

        //We will have it follow PW-350 as long as he is within certain bounds
        if (pw350.transform.position.x >= 0 && pw350.transform.position.x <= 42)
        {
            c.transform.position = new Vector3(pw350.transform.position.x, 0, -10);
        }
    }
}
