using System;
using System.Collections.Generic;
using MusicalRunes;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int coinsAmount;
    public int highScore;
    

    [Serializable]
    private class PowerUpSaveData
    {
        public PowerupType Type;
        public int Level;
    }

    [SerializeField]
    private List<PowerUpSaveData> powerupSaveDataSerializable;
    private Dictionary<PowerupType, PowerUpSaveData> powerupSaveData;

    public string Serialize()
    {
        powerupSaveDataSerializable.Clear();

        foreach(var pair in powerupSaveData)
        {
            powerupSaveDataSerializable.Add(pair.Value);
        }
        return JsonUtility.ToJson(this);
    }

    public static SaveData Deserialize(string jsonString)
    {
        SaveData newSaveData = JsonUtility.FromJson<SaveData>(jsonString);
        foreach(var data in newSaveData.powerupSaveDataSerializable)
        {
            newSaveData.powerupSaveData.Add(data.Type, data);
        }
        
        return newSaveData;
    }

    #region Constructors
    public SaveData()
    {
        powerupSaveDataSerializable = new List<PowerUpSaveData>();
        powerupSaveData = new Dictionary<PowerupType,PowerUpSaveData>();
    }

    public SaveData(bool createDefaults) : this()
    {
        foreach (PowerupType upgradableType in Enum.GetValues(typeof(PowerupType)))
        {
            powerupSaveData[upgradableType] = new PowerUpSaveData
            {
                Type = upgradableType,
                Level = 0
            };
        }
    }

    #endregion

    public int GetUpgradableLevel(PowerupType powerupType)
    {
        return powerupSaveData[powerupType].Level;
    }
    public void SetUpgradable(PowerupType powerupType, int level)
    {
        powerupSaveData[powerupType].Level = level;
    }
}
