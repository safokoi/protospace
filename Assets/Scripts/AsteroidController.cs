using UnityEngine;

public class AsteroidController : MonoBehaviour
{   
    void Update()
    {        
        // Every frame check
        // If asteroid got in blind spot
        if (transform.position.z < -MVC.model.asteroidSpeed)
        {
            // Stop spawning asteroids
            MVC.model.doSpawnAsteroids = false;
        }
    }
}
