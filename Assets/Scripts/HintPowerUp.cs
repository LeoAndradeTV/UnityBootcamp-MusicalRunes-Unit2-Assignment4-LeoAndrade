using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MusicalRunes;

public class HintPowerUp : PowerUp
{
    [SerializeField] private int RuneHintAmount => ((HintPowerUpConfig)powerupConfig).GetHintAmount(currentLevel);

    private List<int> selectedRuneIndexes;
    private bool isActive;
    protected override void PerformPowerupEffect()
    {
        isActive = true;

        GameManager manager = GameManager.Instance;

        selectedRuneIndexes = Enumerable.Range(
            0, manager.BoardRunes.Count).OrderBy(
            index => index == manager.CurrentRuneIndex 
            ? 2 : Random.value).ToList();

        selectedRuneIndexes.RemoveAt(selectedRuneIndexes.Count - 1);
        var selectedHintAmount = System.Math.Min(RuneHintAmount, selectedRuneIndexes.Count);
        selectedRuneIndexes = selectedRuneIndexes.GetRange(0, selectedHintAmount);
        StartCoroutine(AnimateHintPowerUp());
    }

    protected override void OnRuneActivated()
    {
        base.OnRuneActivated();

        if (!isActive)
        {
            return;
        }

        var runes = GameManager.Instance.BoardRunes;
        foreach (var runeIndex in selectedRuneIndexes)
        {
            runes[runeIndex].SetHintVisual(false);
        }
        isActive = false;
        selectedRuneIndexes = null;
    }
    /// <summary>
    /// Animates the hints 
    /// </summary>
    /// <returns></returns>
    private IEnumerator AnimateHintPowerUp()
    {
        // collect the board runes
        var runes = GameManager.Instance.BoardRunes;

        // for each of the selected runes by the player, show the hint for the next rune in the sequence
        for (int i = 0; i < selectedRuneIndexes.Count; i++)
        {
            var runeIndex = selectedRuneIndexes[i];
            var rune = runes[runeIndex];

            if (i == selectedRuneIndexes.Count - 1)
            {
                yield return rune.SetHintVisual(true);
            } else
            {
                rune.SetHintVisual(true);
            }
        }
        
        // as the hint is playing, set player interactivity
        GameManager.Instance.SetPlayerInteractivity(true);
    }
    protected override void SetCooldownBarHeight()
    {
        if (currentLevel == 0)
        {
            cooldownBar.sizeDelta = new Vector2(cooldownBar.sizeDelta.x, cooldownBarHeight);
        }
        else
        {
            base.SetCooldownBarHeight();
        }

    }
}
