using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Model
{
    // Speed of playing shifting 
    public float playerShift = 2;
    // Speed of lerping while moving to direction
    public float playerLerpSpeed = 5;
    //public float playerSpeedBoost = 5;
    // Player rotation angle
    public float playerRotationAngle = 60;
    // Player level play time
    public ReactiveProperty<float> playerTime;
    // Player best level play time
    public ReactiveProperty<float> playerBestTime;
    // Player health points
    public ReactiveProperty<int> playerHealth;
    // Player covered distance
    public ReactiveProperty<float> playerDistance;
    // Asteroid speed on spawn
    public float asteroidSpeed = 100;
    // Asteroid minimum size on spawn
    public float asteroidMinSize = 1;
    // Asteroid maximum size on spawn
    public float asteroidMaxSize = 10;
    // Asteroid rotation on spawn
    public float asteroidRotation = 5;
    // Asteroid spawn time
    public float asteroidSpawnTime = 0.01f;
    // Asteroid spawn line
    public Vector3 asteroidSpawnLine = new Vector3(100, 50, 50);
    // Asteroids' pool
    public List<GameObject> asteroidsPool = new List<GameObject>();
    // Continue to spawn asteroids?
    public bool doSpawnAsteroids = true;
    // Delay between shots
    public float fireRate = 0.15f;
    // Speed of a spawned shot
    public float shotsSpeed = 100;
    // Number of spawned shots
    public float shotsCache = 20;
    // The time determines the moment of next shot
    public float nextShotTime;
    // Shots' pool
    public List<GameObject> shotsPool = new List<GameObject>();

    // Levels data scheme
    // 0 - level passed
    // 1 - current bestTime on level,
    // 2 - best time required on level,
    // 3 - level's asteroid speed on spawn,
    // 4 - level's asteroid max size
    // 5 - level's asteroid spawn time

    // Multidimensional array
    // { easy, normal, hard, insane }
    public float[][] levels = new float[][] { new float[] { 1, 0, 30, 25, 2, 0.1f },  new float[] { 1, 0, 30, 15, 5, 0.01f }, new float[] { 1, 0, 30, 25, 8, 0.0001f }, new float[] { 0, 0, 30, 50, 10, 0.00001f } };

    // Current level player playing on
    public ReactiveProperty<int> currentLevel;

    public Model()
    {
        // Initialize reactive fields        
        currentLevel = new ReactiveProperty<int>(0);
        playerTime = new ReactiveProperty<float>(0);
        playerBestTime = new ReactiveProperty<float>(0);
        playerHealth = new ReactiveProperty<int>(3);
        playerDistance = new ReactiveProperty<float>(0);
    }

    private void subscribeReactiveField()
    {
        // Subscribe render methods to reactive fields value changes
        playerTime
            .ObserveEveryValueChanged(x => x.Value)
            .Subscribe(xs =>
            {
                MVC.view.RenderPlayerTime(xs);
            }).AddTo(MVC.view);

        playerBestTime
            .ObserveEveryValueChanged(x => x.Value)
            .Subscribe(xs =>
            {
                MVC.view.RenderPlayerBestTime(xs);
            }).AddTo(MVC.view);

        playerHealth
            .ObserveEveryValueChanged(x => x.Value)
            .Subscribe(xs =>
            {
                MVC.view.RenderPlayerHealth(xs);
            }).AddTo(MVC.view);

        playerDistance
            .ObserveEveryValueChanged(x => x.Value)
            .Subscribe(xs =>
            {
                MVC.view.RenderPlayerDistance(xs);
            }).AddTo(MVC.view);

        currentLevel
            .ObserveEveryValueChanged(x => x.Value)
            .Subscribe(xs =>
            {
                MVC.view.RenderPlayerGoal(xs);
            }).AddTo(MVC.view);
    }

    public void save()
    {
        // Save model with binary serializer         
        string filename = Application.persistentDataPath + "/model.dat";
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = new FileStream(filename, FileMode.Create);
        ModelData data = new ModelData(this);
        bf.Serialize(fs, data);
        fs.Close();
    }

    public void load()
    {
        // Load model with binary serializer
        string filename = Application.persistentDataPath + "/model.dat";
        if (File.Exists(filename))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(filename, FileMode.Open);
            ModelData data = bf.Deserialize(fs) as ModelData;
            fs.Close();

            this.playerHealth.Value = data.playerHealth;
            this.currentLevel.Value = data.currentLevel;
            this.levels = data.levels;
            this.playerBestTime.Value = data.levels[this.currentLevel.Value][1];            
            this.asteroidSpeed = data.levels[this.currentLevel.Value][3];
            this.asteroidMaxSize = data.levels[this.currentLevel.Value][4];
            this.asteroidSpawnTime = data.levels[this.currentLevel.Value][5];
        }
        // If game view is not initialized 
        // In a case of level chooser menu loaded
        if (MVC.view != null)
        {
            subscribeReactiveField();
        }
    }
}
