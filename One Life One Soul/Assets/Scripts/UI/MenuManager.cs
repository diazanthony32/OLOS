using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private List<Panel> panelHistory = new List<Panel>();
    public Panel defaultPanel;

    [HideInInspector] public Panel currentPanel;

    public static bool GameIsPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        SetUpPanels();
    }

    // used to get all the panels used inside of this menu, and stores them for future use
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
        // allows the player to "go back" for easier menu navigation instead of forcing them to click a button on screen
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoToPrevious();
        }
    }

    // Goes through the opened panel history to allow the player to back out of menus correctly
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

    // adds given panel to the panel history
    public void SetCurrentWithHistory(Panel newPanel)
    {
        panelHistory.Add(currentPanel);
        SetCurrentPanel(newPanel);
    }

    // sets given panel to the active panel the player sees
    void SetCurrentPanel(Panel newPanel)
    {
        if (currentPanel)
        {
            currentPanel.Hide();
        }

        currentPanel = newPanel;
        currentPanel.Show();
    }

    // -------------------------------------------------------------- CUSTOM ACTIONS

    // pauses the game and goes to the pause menu
    void Pause()
    {
        Time.timeScale = 0f;
        GameIsPaused = true;

        Cursor.lockState = CursorLockMode.None;

        SetCurrentWithHistory(defaultPanel);
    }

    // continues the game and removes all panel history
    public void Resume()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;

        currentPanel.Hide();
        currentPanel = null;
        panelHistory.Clear();

        Cursor.lockState = CursorLockMode.Locked;
    }

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
