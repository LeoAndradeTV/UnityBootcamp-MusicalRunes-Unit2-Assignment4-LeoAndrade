using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using MusicalRunes;
using System.Linq;

public class GameManager : MonoBehaviour
{
    private readonly string saveKey = "SaveKey";

    public static GameManager Instance { get; private set; }

    private SaveData saveData;

    [SerializeField] private int roundsCompleted = 0;
    [SerializeField] private int roundsToAddBoard = 5;

    [Header("Rune Settings")]
    private int _initialSequenceSize = 1;
    [SerializeField] private int _initialBoardSize = 4;
    [SerializeField] private RectTransform _runesHolder;
    [SerializeField] private List<Rune> availableRunePrefabs;
    [SerializeField] private float remainingRuneChooseTime = 7f;
    [SerializeField] private float quickBonusTime = 5f;
    [SerializeField] private int extraCoins = 1;
    [SerializeField] private int startLives = 3;
    [SerializeField] private int roundsToShuffleBoard = 5;
    [SerializeField] private TMP_Text remainingTimeText;
    [SerializeField] private TMP_Text remainingLivesText;
    private float startRuneTime;
    private int remainingLives;
    
    private bool choseWrongRune = true;
    public List<Rune> BoardRunes
    {
        get;
        private set;
    }
    private List<Rune> instantiatedBoardRunes;
    /// <summary>
    /// Keep track of the current rune sequence
    /// </summary>
    private List<int> _currentRuneSequence;
    /// <summary>
    /// Current index of the rune that's been played
    /// </summary>
    private int _currentPlayIndex;
    [NonSerialized]
    public bool isRuneChoosingTime;
    public int CurrentRuneIndex => _currentRuneSequence[_currentPlayIndex];
    [SerializeField] private float _delayBetweenRunePreview = 0.3f;

    private void FailedChoice(bool chooseWrongRune = true)
    {
        // implement remaining lives

        // show announcer text
        if (chooseWrongRune)
        {
            if (RemainingLives == 0)
            {
                _announcer.ShowFailedByTimeoutText();
            } else
            {
                _announcer.ShowWrongRuneText();
                RemainingLives--;
            }
            
        } else
        {
            _announcer.ShowTimeOutText();
        }
        // reset rune timer
        if (Mathf.Approximately(remainingRuneChooseTime, 0))
        {
            remainingRuneChooseTime = startRuneTime;
            RemainingLives--;
        }
    }

    public int CoinsAmount
    {
        get => saveData.coinsAmount;
        set
        {
            saveData.coinsAmount = value;
            _coinsAmountText.text = CoinsAmount.ToString();

            // trigger the coint changed action
            coinsChanged?.Invoke(value);
        }
    }
    public int Highscore
    {
        get => saveData.highScore;
        set
        {
            saveData.highScore = value;
            highScoreText.text = Highscore.ToString();
        }
    }

    public int RemainingLives
    {
        get => remainingLives;
        set
        {
            remainingLives = value;
            remainingLivesText.text = remainingLives.ToString();
        }
    }

    [Header("Coin Settings")]
    public int _coinsPerRune = 1;
    [SerializeField] private int _coinsPerRound = 10;

    [Header("Preview Settings")]
    [SerializeField] private GameObject[] _spinlights;

    [Header("Powerup Settings")]
    [SerializeField] private List<PowerUp> powerups;

    [Header("UI Settings")]
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private Announcer _announcer;
    public TextMeshProUGUI _coinsAmountText;

    public Action<int> coinsChanged;
    public Action sequenceCompleted;
    public Action runeActivated;

    public delegate void OnPowerupUpgradedDelegate(PowerupType upgradePowerup, int newLevel);
    public OnPowerupUpgradedDelegate powerupUpgraded;


    private void Update()
    {
        if (isRuneChoosingTime)
        {
            remainingTimeText.text = remainingRuneChooseTime.ToString("F1");
            remainingRuneChooseTime -= Time.deltaTime;
            remainingRuneChooseTime = Mathf.Clamp(remainingRuneChooseTime, 0, startRuneTime);

            if (remainingRuneChooseTime < 3)
            {
                FailedChoice(false);
            }
            if (RemainingLives == 0)
            {
                choseWrongRune = true;
                StartCoroutine(FailedSequence());
            }
        }
    }


    public int GetPowerupLevel(PowerupType powerupType)
    {
        return saveData.GetUpgradableLevel(powerupType);
    }
    public void UpgradePowerup(PowerupType powerupType, int price)
    {
        if (price > CoinsAmount)
        {
            throw new Exception("You are broke, can't buy this");
        } else
        {
            CoinsAmount -= price;

            var newLevel = GetPowerupLevel(powerupType) + 1;
            saveData.SetUpgradable(powerupType, newLevel);

            powerupUpgraded?.Invoke(powerupType, newLevel);
        }
        //PowerupUpgradePopup.Instance.Setup(powerupConfig);
    }

    private void Awake()
    {
        if (Instance != null)
        {
            throw new System.Exception($"Multiple game managers in the scene! {Instance} :: {this}");
        } else
        {
            Instance = this;
        }
        quickBonusTime = remainingRuneChooseTime - 2f;
        startRuneTime = remainingRuneChooseTime;
        LoadData();
        RemainingLives = startLives;
        roundsCompleted = 0;
        CoinsAmount = 0;
        InitializeBoard();
        InitializeSequence();
        InitializeUI();
        StartCoroutine(PlaySequencePreviewCoroutine(2));
    }

    private void InitializeUI()
    {
        highScoreText.text = saveData.highScore.ToString();
        _coinsAmountText.text = CoinsAmount.ToString();
        remainingTimeText.text = remainingRuneChooseTime.ToString("F1");
    }
    // Method to decide if current score is a highscore
    private int FindHighScore(int currentScore, int highscore)
    {
        int result;
        if (currentScore > highscore)
        {
            result = currentScore;
            _announcer.ShowHighScoreText(result);
        } else
        {
            result = highscore;
        }
        return result;
    }
    private void Reset()
    {
        for (int i = _runesHolder.childCount - 1; i >= 0; i--)
        {
            Destroy(_runesHolder.GetChild(i).gameObject);
        }
        availableRunePrefabs.AddRange(instantiatedBoardRunes);
        remainingRuneChooseTime = startRuneTime;
        RemainingLives = startLives;
        remainingTimeText.text = remainingRuneChooseTime.ToString("F1");
        // Reset rounds played
        roundsCompleted = 0;
        // Reset coins amount
        CoinsAmount = 0;
        InitializeBoard();
        InitializeSequence();
    }
    private void AddRandomRuneToBoard()
    {
        var runePrefab = availableRunePrefabs[UnityEngine.Random.Range(0, availableRunePrefabs.Count)];
        availableRunePrefabs.Remove(runePrefab);
        instantiatedBoardRunes.Add(runePrefab);

        var rune = Instantiate(runePrefab, _runesHolder);
        rune.Setup(BoardRunes.Count);
        BoardRunes.Add(rune);
    }
    private void InitializeBoard()
    {
        BoardRunes = new List<Rune>(_initialBoardSize);
        instantiatedBoardRunes = new List<Rune>(_initialBoardSize);

        for (int i = 0; i < _initialBoardSize; i++)
        {
            AddRandomRuneToBoard();
        }
    }
    private void ShuffleBoard()
    {
        var newOrder = Enumerable.Range(0, BoardRunes.Count).OrderBy(_ => UnityEngine.Random.value).ToList();

        BoardRunes = BoardRunes.OrderBy(rune => newOrder.FindIndex(order => order == rune.Index)).ToList();

        for (var sequenceIndex = 0; sequenceIndex < _currentRuneSequence.Count; sequenceIndex++)
        {
            var runeIndex = _currentRuneSequence[sequenceIndex];
            _currentRuneSequence[sequenceIndex] = newOrder.FindIndex(order => order == runeIndex);
        }
        for (var index = 0; index < BoardRunes.Count; index++)
        {
            BoardRunes[index].Setup(index);
        }
    }
    public void OnRuneActivated(int index)
    {
        // TODO: prevent rune clicks when sequence is finished
        if (_currentPlayIndex >= _currentRuneSequence.Count) return;
        if (_currentRuneSequence[_currentPlayIndex] == index)
        {
            CorrectRuneSelected();
        } else
        {
            StartCoroutine(FailedSequence());
        }
    }
    private void InitializeSequence()
    {
        _currentRuneSequence = new List<int>(_initialSequenceSize);
        for (int i = 0; i < _initialSequenceSize; i++)
        {
            _currentRuneSequence.Add(UnityEngine.Random.Range(0, BoardRunes.Count));
        }
    }
    private IEnumerator PlaySequencePreviewCoroutine(float startDelay = 1)
    {
        
        SetPlayerInteractivity(false);
        yield return new WaitForSeconds(startDelay);
        remainingRuneChooseTime = startRuneTime;
        remainingTimeText.text = remainingRuneChooseTime.ToString("F1");
        // TODO: animate each rune in turn
        EnablePreviewFeedback();
        string sequence = "Sequence: ";
        foreach (int index in _currentRuneSequence)
        {
            yield return BoardRunes[index].ActivateRuneCoroutine();
            yield return new WaitForSeconds(_delayBetweenRunePreview);
            sequence += $"{index}, ";
        }
        Debug.Log(sequence);
        DisablePreviewFeedback();
        SetPlayerInteractivity(true);
    }
    public Coroutine PlaySequencePreview(float startDelay = 1,
        bool resetPlayIndex = true)
    {
        if (resetPlayIndex)
        {
            _currentPlayIndex = 0;
        }
        return StartCoroutine(PlaySequencePreviewCoroutine(startDelay));
    }
    public void SetPlayerInteractivity(bool interactable)
    {
        foreach (var rune in BoardRunes)
        {
            if (interactable)
            {
                rune.EnableInteraction();
            } else
            {
                rune.DisableInteraction();
            }
        }
        foreach (var powerup in powerups)
        {
            powerup.Interactable = interactable;
        }
        isRuneChoosingTime = interactable;
    }
    /// <summary>
    /// Sequence has finished and is incorrect
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private IEnumerator FailedSequence()
    {
        // Update the highscore after a failed sequence
        FailedChoice(choseWrongRune);
        if (RemainingLives == 0)
        {
            Highscore = FindHighScore(roundsCompleted, saveData.highScore);
            Save();
            SetPlayerInteractivity(false);
            yield return new WaitForSeconds(2);
            _currentPlayIndex = 0;
            Reset();
            yield return PlaySequencePreviewCoroutine(0);
        }
        
    }
    /// <summary>
    /// When your sequence has finished
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void CompletedSequence()
    {
        
        CoinsAmount += _coinsPerRound;
        // Increase the number of rounds completed after a completed sequence
        roundsCompleted++;
        Save();

        // Check to see if random board rune needs to be added
        if(roundsCompleted % roundsToAddBoard == 0)
        {
            AddRandomRuneToBoard();
        }
        if(roundsCompleted % roundsToShuffleBoard == 0)
        {
            ShuffleBoard();
        }

        // Trigger the sequence completed action
        sequenceCompleted?.Invoke();
        Debug.Log("Completed Sequence");
        _currentRuneSequence.Add(UnityEngine.Random.Range(0, BoardRunes.Count));
        _currentPlayIndex = 0;
        StartCoroutine(PlaySequencePreviewCoroutine(2));
    }
    /// <summary>
    /// When the player has selected the correct rune
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void CorrectRuneSelected()
    {
        bool combo = (startRuneTime - remainingRuneChooseTime) < quickBonusTime;
        remainingRuneChooseTime = startRuneTime;
        runeActivated?.Invoke();
        CoinsAmount += combo ? _coinsPerRune + extraCoins : _coinsPerRune;        
        _currentPlayIndex++;

        if (_currentPlayIndex >= _currentRuneSequence.Count)
        {
            CompletedSequence();
            
        } else
        {
            Save();
        }
    }
    private void EnablePreviewFeedback()
    {
        foreach (var spinlight in _spinlights)
        {
            spinlight.SetActive(true);
        }
        _announcer.ShowPreviewText();
    }
    private void DisablePreviewFeedback()
    {
        foreach (var spinlight in _spinlights)
        {
            spinlight.SetActive(false);
        }
        _announcer.ShowSequenceTest();
    }
    private void LoadData()
    {
        if (PlayerPrefs.HasKey(saveKey))
        {
            string serializedSaveData = PlayerPrefs.GetString(saveKey);
            saveData = SaveData.Deserialize(serializedSaveData);

            return;
        }
        saveData = new SaveData(true);
    }
    private void Save()
    {
        string serializedSaveData = saveData.Serialize();
        PlayerPrefs.SetString(saveKey, serializedSaveData);
        Debug.Log(serializedSaveData);
    }
}
