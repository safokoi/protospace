using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class ModelData
{
    public float playerBestTime;
    public int playerHealth;
    public int currentLevel;
    public float[][] levels;
    
    public ModelData(Model model) 
    {
        this.playerBestTime = model.playerBestTime.Value;
        this.playerHealth = model.playerHealth.Value;        
        this.currentLevel = model.currentLevel.Value;
        this.levels = model.levels;
    }
 
}
