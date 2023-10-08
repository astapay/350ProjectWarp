/*************************************************************************
// File Name:       MenuController.cs
// Author:          Andrew Stapay
// Creation Date:   September 29, 2023
//
// Description:     Controls the menus of the game
*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject controlMenu;
    [SerializeField] private GameObject[] controlSubMenus;
    private int currentControlMenu;
    private int controlMenuChange;

    void Start()
    {
        //Setting resolution and frame rate
        Application.targetFrameRate = 30;
        QualitySettings.vSyncCount = 0;
        Screen.SetResolution(1920, 1080, true);

        currentControlMenu = 0;
        controlMenuChange = 0;
    }

    private void Update()
    {
        if (controlMenu != null)
        {
            controlSubMenus[currentControlMenu].SetActive(false);
            currentControlMenu += controlMenuChange;

            if (currentControlMenu >= 0 && currentControlMenu < controlSubMenus.Length)
            {
                controlSubMenus[currentControlMenu].SetActive(true);
            }
            else
            {
                controlSubMenus[currentControlMenu - controlMenuChange].SetActive(false);
                controlMenu.SetActive(false);
                currentControlMenu = 0;
            }

            controlMenuChange = 0;
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level1");
    }

    public void ControlsMenu()
    {
        currentControlMenu = 0;

        controlMenu.SetActive(true);
        controlSubMenus[currentControlMenu].SetActive(true);
    }

    public void IncrementControlMenu()
    {
        controlMenuChange++;
    }

    public void DecrementControlMenu()
    {
        controlMenuChange--;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void CreditsMenu()
    {
        Blank b = FindObjectOfType<Blank>(true);

        if (b != null)
        {
            b.gameObject.SetActive(true);
        }
    }

    public void BackFromCredits()
    {
        Blank b = FindObjectOfType<Blank>();
        b.gameObject.SetActive(false);
    }

    public void ResumeGame()
    {
        GameManager gm = FindObjectOfType<GameManager>();

        if (gm != null)
        {
            gm.ResumeGame();
        }
    }

    public void RestartLevel()
    {
        ResumeGame();
        Scene s = SceneManager.GetActiveScene();
        SceneManager.LoadScene(s.name);
    }

    public void QuitToMainMenu()
    {
        ResumeGame();
        SceneManager.LoadScene("MainMenu");
    }
}
