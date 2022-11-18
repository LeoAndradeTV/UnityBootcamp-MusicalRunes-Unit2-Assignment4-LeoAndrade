using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MusicalRunes;

[CreateAssetMenu(fileName = "New Treasure Object", menuName = "Configs/TreasureObject")]
public class TreasureConfig : ScriptableObject
{
    public TreasureType treasureType;
    public new string name;
    [TextArea] public string message;
    public int value;
    public float probability;
    public Texture image;
}
