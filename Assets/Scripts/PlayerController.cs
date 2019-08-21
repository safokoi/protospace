using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // IF collision is not with shots
        if (collision.gameObject.name != "shot(Clone)")
        {            
            Controller controller = MVC.controller;
            Model model = MVC.model;
         
            // If player have 1 more health point
            if (model.playerHealth.Value > 1)
            {
                // Decrease health points and reload the game
                controller.decreasePlayerHealth();                
                SceneManager.LoadScene("Game");
            } else
            {
                // Load level chooser menu
                MenuController.menuIndex = MenuController.LEVELCHOOSER;
                SceneManager.LoadScene("Menu");
                controller.restorePlayerHealth();                
                Cursor.visible = true;
            }                        
        }
    }
}
