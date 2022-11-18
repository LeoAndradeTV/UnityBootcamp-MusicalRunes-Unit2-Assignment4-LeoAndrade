using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MusicalRunes;

public abstract class PowerUp : MonoBehaviour
{
    [SerializeField] protected PowerupConfig powerupConfig;
    [SerializeField] private Button powerUpButton;
    [SerializeField] protected RectTransform cooldownBar;

    protected float cooldownBarHeight;
    private int currentCooldown;
    protected int currentLevel;
    private int cooldownDuration => powerupConfig.GetCooldown(currentLevel);

    public bool Interactable
    {
        get => powerUpButton.interactable;
        set => powerUpButton.interactable = isAvailable && true;
    }

    protected virtual bool isAvailable => currentLevel > 0 && currentCooldown <= 0;

    void Start()
    {
        currentLevel = GameManager.Instance.GetPowerupLevel(powerupConfig.powerupType);
        currentCooldown = cooldownDuration;
        Interactable = isAvailable;
        // grabbing the original height of the cooldown bar and using it as the max value for the bar height
        cooldownBarHeight = cooldownBar.sizeDelta.y;
        // setting the cooldown bar height
        SetCooldownBarHeight(); 
        // pavlov's dog action conditioning
        powerUpButton.onClick.AddListener(OnClick);
        GameManager.Instance.sequenceCompleted += OnSequenceCompleted;
        GameManager.Instance.runeActivated += OnRuneActivated;
    }

    protected abstract void PerformPowerupEffect();

    private void OnPowerupUpgraded(PowerupType upgradedPowerup, int newLevel)
    {
        if(upgradedPowerup != powerupConfig.powerupType)
            return;

        currentLevel = newLevel;
        Interactable = false;
    }

    private void OnClick()
    {
        // reset the cooldown
        ResetCooldown();
        // mark the powerup button as unavailable and not interactable
        Interactable = false;
        // when player clicks on powerup, perform powerup
        PerformPowerupEffect();
        // if the player has clicked on the powerup and it's not available, say something
        Debug.Assert(isAvailable, "Power up unavailable", gameObject);
    }

    private void ResetCooldown()
    {
        // return the current cooldown to the original cooldown duration value
        currentCooldown = cooldownDuration;
        // set the cooldown bar height
        SetCooldownBarHeight();
    }
    protected virtual void OnSequenceCompleted()
    {
        // check to see if rune is activated
        if (powerupConfig.decreaseCooldownOnRuneActivation)
        {
            return;
        }
        DecreaseCooldown();
    }
    protected virtual void OnRuneActivated()
    {
        if (!powerupConfig.decreaseCooldownOnRuneActivation)
        {
            return;
        }
        DecreaseCooldown();
    }

    private void DecreaseCooldown()
    {
        // check if cooldown is available
        if (isAvailable) { return; }
        // decrease the cooldown
        currentCooldown--;
        // additional check: make sure current cooldown is between 0 and currentcooldown value
        currentCooldown = Mathf.Max(0, currentCooldown);
        // set the bar height visual
        SetCooldownBarHeight();
        // when cooldown is unavailable/available, set it's interactivity
        Interactable = isAvailable;
    }

    protected virtual void SetCooldownBarHeight()
    {
        // normalizing the height from 0 to 1
        var fraction = (float) currentCooldown / cooldownDuration;

        // apply the fraction we calculated to the cooldownBarHeight
        cooldownBar.sizeDelta = new Vector2(cooldownBar.sizeDelta.x, fraction * cooldownBarHeight);
    }

    private void OnDestroy()
    {
        GameManager.Instance.sequenceCompleted -= OnSequenceCompleted;
        GameManager.Instance.runeActivated -= OnRuneActivated;
    }
}
