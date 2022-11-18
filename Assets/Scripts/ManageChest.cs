using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MusicalRunes;
using TMPro;
using System;

public class ManageChest : MonoBehaviour
{
    [SerializeField] private GameObject treasureInfo;
    [SerializeField] private Button openChest;
    [SerializeField] private TMP_Text treasureName;
    [SerializeField] private TMP_Text message;
    [SerializeField] private TMP_Text treasureValue;
    [SerializeField] private RawImage treasureImage;
    [SerializeField] private Button claimPrize;
    [SerializeField] private List<TreasureConfig> allConfigs;

    private TreasureConfig treasureConfig;
    private float probability;

    private void Awake()
    {
        treasureInfo.SetActive(false);
    }

    public void SetUpChestMenu(TreasureConfig config)
    {
        treasureConfig = config;
        probability = treasureConfig.probability;
        treasureName.text = treasureConfig.name;
        treasureValue.text = treasureConfig.value.ToString();
        message.text = treasureConfig.message;
        treasureImage.texture = treasureConfig.image;
        
    }

    public void OnTreasureClick()
    {
        TreasureConfig treasureConfig = GetRandomTreasure();
        SetUpChestMenu(treasureConfig);
        claimPrize.onClick.AddListener(OnClaimClicked);
        treasureInfo.SetActive(true);
    }

    private TreasureConfig GetRandomTreasure()
    {
        TreasureConfig returnConfig = ScriptableObject.CreateInstance<TreasureConfig>();
        var values = Enum.GetValues(typeof(TreasureType));
        List<TreasureType> probability = new List<TreasureType>();
        foreach(var config in allConfigs)
        {
            float prob = config.probability;
            for (int i = 0; i < prob; i++)
            {
                probability.Add(config.treasureType);
            }
        }
        int randomTreasure = UnityEngine.Random.Range(0, probability.Count);
        TreasureType type = probability[randomTreasure];
        foreach (var config in allConfigs)
        {
            if (config.treasureType == type)
            {
                returnConfig = config;
            }
        }
        return returnConfig;
    }

    private void OnClaimClicked()
    {
        GameManager.Instance.CoinsAmount += treasureConfig.value;
        claimPrize.onClick.RemoveAllListeners();
        treasureInfo.SetActive(false);
    }
    
}
