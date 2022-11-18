using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayPowerUp : PowerUp
{
    [SerializeField] private int runesToActivatePowerUp = 2;

    [SerializeField] private int runesActivatedInCurrentSequence;

    private int cooldownBarHeightFactor = 1;
    protected override bool isAvailable => runesActivatedInCurrentSequence >= runesToActivatePowerUp;

    protected override void PerformPowerupEffect()
    {
        GameManager.Instance.PlaySequencePreview();
        ResetRuneBehavior();
    }

    private void ResetRuneBehavior()
    {
        runesActivatedInCurrentSequence = 0;
        cooldownBarHeightFactor = 1;
        SetCooldownBarHeight();
        Interactable = isAvailable;
    }

    protected override void OnRuneActivated() 
    { 
        runesActivatedInCurrentSequence++;
        if (runesActivatedInCurrentSequence >= runesToActivatePowerUp)
        {
            cooldownBarHeightFactor = 0;
            SetCooldownBarHeight();
            
        } else
        {
            cooldownBarHeightFactor = 1;
            SetCooldownBarHeight();
        }
        Interactable = isAvailable;
    }

    protected override void OnSequenceCompleted()
    {
        ResetRuneBehavior();
    }

    protected override void SetCooldownBarHeight()
    {
        cooldownBar.sizeDelta = new Vector2(cooldownBar.sizeDelta.x, cooldownBarHeight * cooldownBarHeightFactor);
    }
}
