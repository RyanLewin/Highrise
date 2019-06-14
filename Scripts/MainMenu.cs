using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject doesNothingUI;

    private void Start()
    {
        doesNothingUI.SetActive(false);
    }

    /// <summary>Lauches the "City Scene"</summary>
    public void NewGame()
    {
        SceneManager.LoadScene("City", LoadSceneMode.Single);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("City", LoadSceneMode.Single);
    }

    /// <summary>Exit Application</summary>
    public void ExitGame()
    {
        Application.Quit();
    }

    public void SwapMenu()
    {
        gameObject.GetComponent<PauseMenu>().SwapMenus();
    }

    public void ToggleDoesNothing()
    {
        doesNothingUI.SetActive(!doesNothingUI.activeSelf);
    }
}
