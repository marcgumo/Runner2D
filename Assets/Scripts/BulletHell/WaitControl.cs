using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitControl : MonoBehaviour
{
    public GameObject waitPanel;
    private bool isPaused;

    void Start()
    {
        isPaused = false;
        waitPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetPause();
        }
    }

    private void SetPause()
    {
        isPaused = !isPaused;
        waitPanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void Continue()
    {
        SetPause();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
