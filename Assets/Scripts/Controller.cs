using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller
{
    // Initialize the model and view
    Model model = MVC.model;
    View view = MVC.view;

    public IEnumerator spawnAsteroids()
    {        
        // Spawn asteroids until one of it get to blind point
        // doSpawnAsteroids is setted by AsteroidController
        while (model.doSpawnAsteroids)
        {
            // Random poistion on asteroid spawn line
            Vector3 spawnPosition = new Vector3(Random.Range(-model.asteroidSpawnLine.x+view.player.transform.position.x, model.asteroidSpawnLine.x + view.player.transform.position.x), Random.Range(-model.asteroidSpawnLine.y + view.player.transform.position.y, model.asteroidSpawnLine.y+view.player.transform.position.y), model.asteroidSpawnLine.z);            
            // Instantiate an asteroid with one of specified models, position and rotation
            GameObject asteroid = View.Instantiate(view.asteroids[Random.Range(0, 3)], spawnPosition, Quaternion.identity);
            // Change initial color of the asteroid to random rgb color;
            asteroid.GetComponent<Renderer>().material.color = new Color((float)Random.Range(0, 255) / 255, (float)Random.Range(0, 255) / 255, (float)Random.Range(0, 255) / 255);
            // Determine size of the asteroid
            float size = Random.Range(model.asteroidMinSize, model.asteroidMaxSize);
            // Set size of the asteroid
            asteroid.transform.localScale = new Vector3(size, size, size);
            // Set initial velocity of asteroid
            asteroid.GetComponent<Rigidbody>().velocity = Vector3.forward * -model.asteroidSpeed;            
            // Set initial rotation of asteroid
            asteroid.GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * model.asteroidRotation;
            // Add the asteroid to asteroids' pool
            model.asteroidsPool.Add(asteroid);
            // Wait for next spawn
            yield return new WaitForSeconds(model.asteroidSpawnTime);
        } 
        
        // When all asteroids spawned loop forever
        while (true)
        {
            // Loop through each asteroid
            foreach (var asteroid in model.asteroidsPool)
            {
                // If an asteroid is entered in blind spot
                // Blind spot is located at -15 behind player
                if (asteroid.transform.position.z < -15)
                {
                    // Set random position of the asteroid on spawn line
                    asteroid.transform.position = new Vector3(Random.Range(-model.asteroidSpawnLine.x + view.player.transform.position.x, model.asteroidSpawnLine.x + view.player.transform.position.x), Random.Range(-model.asteroidSpawnLine.y + view.player.transform.position.y, model.asteroidSpawnLine.y + view.player.transform.position.y), model.asteroidSpawnLine.z);
                    // Determine size of the asteroid
                    float size = Random.Range(model.asteroidMinSize, model.asteroidMaxSize);
                    // Set the size of the asteroid
                    asteroid.transform.localScale = new Vector3(size, size, size);
                    // Set the velocity of the asteroid specified by asteroid speed
                    // Minus value caused by opposite direction relative to the player                    
                    asteroid.GetComponent<Rigidbody>().velocity = Vector3.forward * -model.asteroidSpeed;                    
                }
            }
            // Wait for next spawn
            yield return new WaitForSeconds(model.asteroidSpawnTime);
        }
    }

    public void playerFire()
    { 
        // Set initial position and rotation of a shot in front of player
        // with shift of 1 to prevent colliding with player.
        Vector3 spawnPosition = view.player.transform.position + Vector3.forward * 1f;              

        //Instantiate shots until number is reached poolCache value.
        if (model.shotsPool.Count<20) {             
            // Instantiate a shot
            GameObject shot = View.Instantiate(view.shot, spawnPosition, Quaternion.identity);            
            // Set shot velocity
            shot.GetComponent<Rigidbody>().velocity = view.player.transform.forward * model.shotsSpeed;
            // Add the shot to  a shots' pool
            model.shotsPool.Add(shot);            
        } else
        {
            //Get the farest shot in the pool
            GameObject shot = model.shotsPool[0];
            // Remove the shot from the pool
            model.shotsPool.Remove(shot);
            // Append the shot to the end of pool
            model.shotsPool.Add(shot);
            // Change position to initial spawn
            shot.transform.position = spawnPosition;
            // Change velocity of the shot
            shot.GetComponent<Rigidbody>().velocity = view.player.transform.forward * model.shotsSpeed;
        }

        // Set a delay for next shot
        model.nextShotTime = Time.time + model.fireRate;
    }

    public void playerMoveForward()
    {
        // Lerping a player to the forward direction along with camera
        view.player.transform.rotation = Quaternion.Lerp(view.player.transform.rotation, Quaternion.Euler(new Vector3(0, 0, 0)), model.playerLerpSpeed * Time.deltaTime);
        view.camera.transform.position = Vector3.Lerp(view.camera.transform.position, view.player.transform.position + new Vector3(0, 0, view.camera.transform.position.z-view.player.transform.position.z), model.playerLerpSpeed * Time.deltaTime);
    }

    public void playerMove(Vector3 direction)
    {
        // Lerping position of a player, camera, stars particles system and asteroid spawn line in a specified direction
        // Camera is lerping with a little delay
        view.camera.transform.position = Vector3.Lerp(view.camera.transform.position, view.camera.transform.position + direction, (model.playerLerpSpeed-2) * Time.deltaTime);
        view.player.transform.position = Vector3.Lerp(view.player.transform.position, view.player.transform.position + direction, model.playerLerpSpeed * Time.deltaTime);
        view.stars.transform.position = Vector3.Lerp(view.stars.transform.position, view.stars.transform.position + direction, model.playerLerpSpeed * Time.deltaTime);
        model.asteroidSpawnLine = Vector3.Lerp(model.asteroidSpawnLine, model.asteroidSpawnLine + direction, model.playerLerpSpeed * Time.deltaTime);
            
        playerRotate(direction);
    }

    public void playerRotate(Vector3 direction)
    {
        // Lerping player rotation in specified direction
        // y>0 - upward, y<0 - downward  
        if (direction.y > 0) view.player.transform.rotation = Quaternion.Lerp(view.player.transform.rotation, Quaternion.Euler(new Vector3(view.player.transform.rotation.eulerAngles.x - model.playerRotationAngle, 0, 0)), model.playerLerpSpeed * Time.deltaTime);
        else if (direction.y < 0) view.player.transform.rotation = Quaternion.Lerp(view.player.transform.rotation, Quaternion.Euler(new Vector3(view.player.transform.rotation.eulerAngles.x + model.playerRotationAngle, 0, 0)), model.playerLerpSpeed * Time.deltaTime);
        // x>0 - to the right, x<0 - to the left
        if (direction.x > 0) view.player.transform.rotation = Quaternion.Lerp(view.player.transform.rotation, Quaternion.Euler(new Vector3(0, view.player.transform.rotation.eulerAngles.y + model.playerRotationAngle, 0)), model.playerLerpSpeed * Time.deltaTime);
        else if (direction.x < 0) view.player.transform.rotation = Quaternion.Lerp(view.player.transform.rotation, Quaternion.Euler(new Vector3(0, view.player.transform.rotation.eulerAngles.y - model.playerRotationAngle, 0)), model.playerLerpSpeed * Time.deltaTime);
    }

    public void setPlayerDistance(float playerDistance)
    {        
        // Set the covered distance 
        model.playerDistance.Value += playerDistance * Time.deltaTime;
    }

    public void restorePlayerHealth()
    {
        // Set the player health points 
        MVC.model.playerHealth.Value = 3;
        // Persist model to prevent data loss due to scene reload
        model.save();
    }

    public void decreasePlayerHealth()
    {
        // Decrease player health point by 1
        model.playerHealth.Value--;
        // Persist model to prevent data loss due to scene reload
        model.save();
    }
    
    public void updateView(float deltaTime)
    {
        // Update View on every frame
        // Renew player time
        model.playerTime.Value += deltaTime;
        // If player time is the best
        if (model.playerTime.Value > model.playerBestTime.Value)
        {
            // Renew best time value
            model.playerBestTime.Value = model.playerTime.Value;            
            model.levels[model.currentLevel.Value][1] = model.playerBestTime.Value;
        }
        // If player achieved the level goal
        if (model.levels[model.currentLevel.Value][2] <= model.playerBestTime.Value)
        {
            // Level is passed
            levelPassed();
        }
    }

    private void levelPassed()
    {
        // Mark current level passed 
        // 0 element - level passed
        // 1 element - level's player best time
        model.levels[model.currentLevel.Value][0] = 1;
        model.levels[model.currentLevel.Value][1] = 0;
       
        loadLevelMenu();
    }

    public void loadLevelMenu()
    {
        // Load level chooser menu
        // Persist model to prevent data loss due to scene reload
        model.save();
        // Set cursor visible
        Cursor.visible = true;
        // Load the level chooser menu
        MenuController.menuIndex = MenuController.LEVELCHOOSER;
        SceneManager.LoadScene("Menu");
    }

}
