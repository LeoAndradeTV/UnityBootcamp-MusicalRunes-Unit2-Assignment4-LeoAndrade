using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MusicalRunes;

public class ExtraLivesPowerUp : PowerUp
{
 
    protected override void PerformPowerupEffect()
    {
        Debug.Log("Powerup is being clicked");
        GameManager.Instance.RemainingLives++;
    }

    protected override void SetCooldownBarHeight()
    {
        if(currentLevel == 0)
        {
            cooldownBar.sizeDelta = new Vector2(cooldownBar.sizeDelta.x, cooldownBarHeight);
        } else
        {
            base.SetCooldownBarHeight();
        }
        
    }

}
