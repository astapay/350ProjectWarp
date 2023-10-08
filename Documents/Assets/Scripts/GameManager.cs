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
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using TMPro;

public class GameManager : MonoBehaviour
{
    // The GameObject of the main character. Used to keep track of where the camera should be.
    [SerializeField] private GameObject pw350;

    // In-game UI elements
    [SerializeField] private GameObject healthBarJuice;
    [SerializeField] private GameObject portalBarJuice;

    // Menus to produces
    [SerializeField] private GameObject levelClear;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;

    // A frame counter used to keep track of how many frames to stop time
    private int freezeCounter;

    // <summary>
    // Start is called before the first frame update
    // Sets the resolution and framerate for the game
    // </summary>
    void Start()
    {
        //Setting resolution and frame rate
        Application.targetFrameRate = 30;
        QualitySettings.vSyncCount = 0;
        Screen.SetResolution(1920, 1080, true);
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

        float xPos = pw350.transform.position.x;
        float yPos = pw350.transform.position.y;

        if (xPos < 0)
        {
            xPos = 0;
        }
        if (yPos < 0)
        {
            yPos = 0;
        }

        if (SceneManager.GetActiveScene().name == "Level1")
        {
            if (xPos > 42)
            {
                xPos = 42;
            }

            yPos = 0;
        }
        else if (SceneManager.GetActiveScene().name == "Level2")
        {
            if (xPos > 90)
            {
                xPos = 90;
            }

            if (yPos > 25)
            {
                yPos = 25;
            }
        }

        c.transform.position = new Vector3(xPos, yPos, -10);

        // Decrement our freeze counter
        freezeCounter--;

        // If we are done with freeze frames, then we unfreeze the game
        if(freezeCounter == 0)
        {
            UnfreezeTime();
        }
    }

    public void FreezeTime()
    {
        Time.timeScale = 0;
    }

    private void UnfreezeTime()
    {
        Time.timeScale = 1;
    }

    public bool TimeFrozen()
    {
        return Time.timeScale == 0;
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

    public void UpdateHealthJuice(int hp)
    {
        float xPos = (hp * 5) / 2.0f;
        float xScale = hp / 50.0f;

        float yPos = healthBarJuice.transform.position.y;
        float yScale = healthBarJuice.transform.localScale.y;

        healthBarJuice.transform.position = new Vector2(xPos, yPos);
        healthBarJuice.transform.localScale = new Vector2(xScale, yScale);
    }

    public void UpdatePortalJuice(int gems)
    {
        float xPos = (gems * 83) / 2.0f;
        float xScale = gems / 3.0f;

        if (SceneManager.GetActiveScene().name == "Level2")
        {
            xScale /= 3;
        }

        float yPos = portalBarJuice.transform.position.y;
        float yScale = portalBarJuice.transform.localScale.y;

        portalBarJuice.transform.position = new Vector2(xPos, yPos);
        portalBarJuice.transform.localScale = new Vector2(xScale, yScale);
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        FreezeTime();
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        UnfreezeTime();
    }

    public void LevelClear()
    {
        levelClear.SetActive(true);
    }

    public void NextLevel()
    {
        Scene s = SceneManager.GetActiveScene();
        SceneManager.LoadScene(s.buildIndex + 1);
    }

    public void GameOver()
    {
        gameOverMenu.SetActive(true);
    }
}
