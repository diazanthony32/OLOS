using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private List<Panel> panelHistory = new List<Panel>();
    public Panel defaultPanel;

    [HideInInspector]
    public Panel currentPanel;

    public static bool GameIsPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        SetUpPanels();
    }

    void SetUpPanels()
    {
        Panel[] panels = GetComponentsInChildren<Panel>();

        foreach (Panel panel in panels)
        {
            panel.SetUp(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoToPrevious();
        }
    }

    public void GoToPrevious()
    {
        if (panelHistory.Count == 0 && !GameIsPaused)
        {
            Pause();
            return;
        }
        else if (currentPanel == defaultPanel && GameIsPaused)
        {
            Resume();
            return;
        }

        int lastIndex = panelHistory.Count - 1;
        SetCurrentPanel(panelHistory[lastIndex]);
        panelHistory.RemoveAt(lastIndex);
    }

    public void SetCurrentWithHistory(Panel newPanel)
    {
        panelHistory.Add(currentPanel);
        SetCurrentPanel(newPanel);
    }

    void SetCurrentPanel(Panel newPanel)
    {
        if (currentPanel)
        {
            currentPanel.Hide();
        }

        currentPanel = newPanel;
        currentPanel.Show();
    }

    void Pause()
    {
        Time.timeScale = 0f;
        GameIsPaused = true;

        SetCurrentWithHistory(defaultPanel);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;

        currentPanel.Hide();
        currentPanel = null;
        panelHistory.Clear();
    }

    // -------------------------------------------- ACTIONS

    public void ReloadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
            Application.Quit();
    }
}
