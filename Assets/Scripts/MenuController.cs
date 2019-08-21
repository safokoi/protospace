using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{   
    public static int MAINMENU = 0;
    public static int LEVELCHOOSER = 1;
    // Menu index
    public static int menuIndex = 0;

    // Buttons for choosing levels initialized in editor
    public Button easyLevelButton;
    public Button normalLevelButton;
    public Button hardLevelButton;
    public Button insaneLevelButton;

    // Menus on canvas initialized in editor
    public GameObject mainMenu;
    public GameObject levelMenu;

    Controller controller;
    Model model;

    private void Awake()
    {
        controller = new Controller();          
        model = new Model();
        
        model.load();
        
        if (model.levels[0][0] == 1)
        {
            easyLevelButton.image.color = new Color(0, 255, 0);
            normalLevelButton.interactable = true;
        }
        if (model.levels[1][0] == 1)
        {
            normalLevelButton.image.color = new Color(0, 255, 0);
            hardLevelButton.interactable = true;
        }
        if (model.levels[2][0] == 1)
        {
            hardLevelButton.image.color = new Color(0, 255, 0);
            insaneLevelButton.interactable = true;
        }
        if (model.levels[3][0] == 1)
        {
            insaneLevelButton.image.color = new Color(0, 255, 0);
        }
        //   }
    }

    private void Start()
    {        
        if (menuIndex == LEVELCHOOSER)
        {
            // Render level chooser menu
            levelMenu.SetActive(true);
            mainMenu.SetActive(false);
        }
        if (menuIndex == MAINMENU)
        {
            // Render main menu
            levelMenu.SetActive(false);
            mainMenu.SetActive(true);
        }
    }

    public void LoadGame(int level)
    {
        // Setting current level to the model and load the game
        model.load();
        model.currentLevel.Value = level;
        model.save();
        SceneManager.LoadScene("Game");
    }

    public void ExitGame()
    {
        // Quit game 
        Application.Quit();
    }
}
