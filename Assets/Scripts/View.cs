using UnityEngine;
using UnityEngine.UI;

public class View : MonoBehaviour
{
    // Interface objects initialized in editor
    // UI Health points text field
    public Text playerHealthText;
    // UI Time text field
    public Text playerTimeText;
    // UI Best time text field;
    public Text playerBestTimeText;
    // UI Distance text field
    public Text playerDistanceText;
    // UI Goal text field
    public Text playerGoalText;
    // Player model
    public GameObject player;
    // Shot model
    public GameObject shot;
    // Array of asteroid models
    public GameObject[] asteroids;
    // Main camera
    public Camera camera;
    // Stars particle system
    public ParticleSystem stars;

    // Declate MVC fields
    private View view;
    private Controller controller;
    private Model model;

    private void Awake()
    {        
        // Initializing MVC fields on view instancing.        
        view = this;
        MVC.view = view;

        model = new Model();
        MVC.model = model;

        controller = new Controller();
        MVC.controller = controller;
    }
    
    void Start()
    {
        
        // Load model data from file on game start
        model.load();        
        // Turn off cursor
        Cursor.visible = false;
        // Start spawning asteroids
        StartCoroutine(controller.spawnAsteroids());
    }

    // Update is called once per frame
    void Update()
    {
        controller.updateView(Time.deltaTime);
        Navigate();
    }

    private void Navigate()
    {
        // Check user input and set keymap
        int keymap = 0;
        if (Input.GetKey(KeyCode.W) | Input.GetKey(KeyCode.UpArrow)) keymap += 1;
        if (Input.GetKey(KeyCode.A) | Input.GetKey(KeyCode.LeftArrow)) keymap += 10;
        if (Input.GetKey(KeyCode.D) | Input.GetKey(KeyCode.RightArrow)) keymap += 100;
        if (Input.GetKey(KeyCode.S) | Input.GetKey(KeyCode.DownArrow)) keymap += 1000;

        // Initialize direction variable in a case it won't be changed in switch block
        Vector3 direction = new Vector3(0, 0, 0);

        // Deterimine moving direction of a player based on keymap
        switch (keymap)
        {
            case 1:
                // Up
                direction = new Vector3(0, model.playerShift, 0);
                break;
            case 10:
                // Left
                direction = new Vector3(-model.playerShift, 0, 0);
                break;
            case 11:
                // Left, Up
                direction = new Vector3(Mathf.Sin(45 * Mathf.PI / 180) * (-model.playerShift), Mathf.Sin(45 * Mathf.PI / 180) * model.playerShift, 0);
                break;
            case 100:
                // Right
                direction = new Vector3(model.playerShift, 0, 0);
                break;
            case 101:
                // Right, Up
                direction = new Vector3(Mathf.Sin(45 * Mathf.PI / 180) * model.playerShift, Mathf.Sin(45 * Mathf.PI / 180) * model.playerShift, 0);
                break;
            case 110:
                // Left, Right;
                break;
            case 111:
                // Left, Right, Up
                break;
            case 1000:
                // Down
                direction = new Vector3(0, -model.playerShift, 0);
                break;
            case 1001:
                // Up, Down
                break;
            case 1010:
                // Left, Down
                direction = new Vector3(Mathf.Sin(45 * Mathf.PI / 180) * (-model.playerShift), Mathf.Sin(45 * Mathf.PI / 180) * (-model.playerShift), 0);
                break;
            case 1011:
                // Right, Left, Up
                break;
            case 1100:
                // Right, Down
                direction = new Vector3(Mathf.Sin(45 * Mathf.PI / 180) * model.playerShift, Mathf.Sin(45 * Mathf.PI / 180) * (-model.playerShift), 0);
                break;
            case 1101:
                // Right, Down, Up
                break;
            case 1110:
                // Right, Left, Down
                break;
            case 1111:
                // Right, Left, Up, Down;
                break;
            default: break;
        }

        // Move player forward
        controller.playerMoveForward();
        // Move player in specified by user direction
        controller.playerMove(direction);

        // Determine distance covered by player
        if (keymap != 0)
        {
            // If player turning any direction
            controller.setPlayerDistance(Mathf.Tan((90 - model.playerRotationAngle) * Mathf.PI / 180) * model.playerShift);
        }
        else
        {
            // If player go straight
            controller.setPlayerDistance(model.playerShift);
        }

        // If user press space
        if (Input.GetKey(KeyCode.Space))
        {
            // If firespeed is under fire rate
            if (model.nextShotTime < Time.time)
            {
                // Fire
                controller.playerFire();
            }
        }

        // If player press esc
        if (Input.GetKey(KeyCode.Escape))
        {
            // Load level chooser menu            
            controller.loadLevelMenu();
        }
    }

    public void RenderPlayerHealth(int health)
    {
        // Render health points
        playerHealthText.text = "Health: " + health.ToString();
    }

    public void RenderPlayerTime(float time)
    {
        // Render play time
        playerTimeText.text = "Time: " + time.ToString("f2");
    }

    public void RenderPlayerBestTime(float time)
    {
        // Render best time
        playerBestTimeText.text = "Best time: " + time.ToString("f2");
    }

    public void RenderPlayerDistance(float distance)
    {
        // Render covered distance
        playerDistanceText.text = "Distance: " + distance.ToString("f2");
    }

    public void RenderPlayerGoal(int currentLevel)
    {
        // Render current goal
        playerGoalText.text = "Goal: " + model.levels[currentLevel][2].ToString();
    }
}
