using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Announcer : MonoBehaviour, ILocalizable
{
    [SerializeField] private TMP_Text announcerText;
    [SerializeField] private float _bounceAmplitude = 0.01f;
    [SerializeField] private float _bounceFrequency = 5f;
    private string previewTextID = "AnnouncerListen";
    private string sequenceInputTextID = "AnnouncerPlayerTurn";
    private string timeoutTextID = "AnnouncerTimeout";
    private string wrongRuneTextID = "AnnouncerFailedChoice";
    private string highScoreTextID = "AnnouncerHighScore";
    private string failedByTimeoutTextID = "AnnouncerFailedByTimeout";
    private string failedByRuneChoiceID = "AnnouncerFailedByRuneChoice";
    private string bloodSacrificeID = "AnnouncerBloodSacrifice";

    private string currentTextID;
    private bool mustFormat;
    private string formatParam;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.one + Vector3.one * (Mathf.Sin(Time.time * _bounceFrequency) * _bounceAmplitude);
    }

    public void ShowPreviewText()
    {
        currentTextID = previewTextID;
        mustFormat = false;
        UpdateText();
    }

    public void ShowSequenceTest()
    {
        currentTextID = sequenceInputTextID;
        mustFormat = false;
        UpdateText();
    }

    public void Clear()
    {
        announcerText.text = "";
    }

    public void ShowWrongRuneText()
    {
        currentTextID = wrongRuneTextID;
        mustFormat = false;
        UpdateText();
    }
    public void ShowHighScoreText(int highScore)
    {
        currentTextID = highScoreTextID;
        mustFormat = true;
        formatParam = highScore.ToString();
        UpdateText();
    }

    public void ShowTimeOutText()
    {
        currentTextID = timeoutTextID;
        mustFormat = false;
        UpdateText();
    }

    public void ShowFailedByTimeoutText()
    {
        currentTextID = failedByTimeoutTextID;
        mustFormat = false;
        UpdateText();
    }

    public void ShowBloodSacrificeText()
    {
        currentTextID = bloodSacrificeID;
        mustFormat = false;
        UpdateText();
    }

    private void UpdateText()
    {
        if (currentTextID == String.Empty)
        {
            announcerText.text = String.Empty;
        }
        else if (mustFormat) 
        {
            announcerText.text = String.Format(Localization.GetLocalizedText(highScoreTextID), formatParam);
        } else
        {
            announcerText.text = Localization.GetLocalizedText(currentTextID);
        }
    }

    public void LocaleChanged()
    {
        UpdateText();
    }

    private void Awake()
    {
        Localization.RegisterWatcher(this);
    }
    private void OnDestroy()
    {
        Localization.DeregisterWatcher(this);
    }
}
