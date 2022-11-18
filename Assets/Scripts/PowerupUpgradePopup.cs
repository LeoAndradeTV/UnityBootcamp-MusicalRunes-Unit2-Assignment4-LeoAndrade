using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace MusicalRunes
{
    public class PowerupUpgradePopup : MonoBehaviour, ILocalizable, IPointerDownHandler
    {
        public static PowerupUpgradePopup Instance { get; private set; }

        private SaveData saveData;

        //[SerializeField] private PowerupConfig powerupConfig;
        [SerializeField] private TMP_Text powerupName;
        [SerializeField] private TMP_Text powerupLevel;
        [SerializeField] private TMP_Text powerupDescription;
        [SerializeField] private TMP_Text powerupPrice;
        [SerializeField] Image coinIconImage;
        [SerializeField] Image purchaseButtonImage;
        [SerializeField] Button purchaseButton;

        public Color purchaseAvailableTextColor = new Color(80, 220, 65);
        public Color purchaseDisabledTextColor = new Color(230, 75, 90);
        public Color purchaseDisabledButtonColor = new Color(170, 170, 170);

        private PowerupConfig config;
        private int currentLevel;

        public void Setup(PowerupConfig powerupConfig)
        {
            config = powerupConfig;
            currentLevel = GameManager.Instance.GetPowerupLevel(config.powerupType);
            powerupName.text = config.PowerUpName;
            powerupLevel.text = currentLevel.ToString();
            powerupDescription.text = config.Description;
            powerupPrice.text = config.GetUpgradePrice(currentLevel).ToString();

            var hasEnoughCoins = GameManager.Instance.CoinsAmount >= config.GetUpgradePrice(currentLevel);
            powerupPrice.color = hasEnoughCoins ? purchaseAvailableTextColor : purchaseDisabledTextColor;
            purchaseButton.interactable = hasEnoughCoins;

            var tintColor = hasEnoughCoins ? Color.white : purchaseDisabledTextColor;
            purchaseButtonImage.color = tintColor;
            coinIconImage.color = tintColor;
            gameObject.SetActive(true);
            purchaseButton.gameObject.SetActive(config.MaxLevel != currentLevel);
            
            GameManager.Instance.isRuneChoosingTime = false;

        }

        public void ClosePopup()
        {
            gameObject.SetActive(false);
            GameManager.Instance.isRuneChoosingTime = true;
        }
        private void OnClick()
        {
            GameManager.Instance.UpgradePowerup(config.powerupType, config.GetUpgradePrice(currentLevel));
            GameManager.Instance.isRuneChoosingTime = true;
            ClosePopup();
        }
        private void Awake()
        {
            purchaseButton.onClick.AddListener(OnClick);
            gameObject.SetActive(false);
            Localization.RegisterWatcher(this);
        }
        private void OnDestroy()
        {
            Localization.DeregisterWatcher(this);
        }

        public void LocaleChanged()
        {
            if (config == null) { return; }
            powerupName.text = config.PowerUpName;
            powerupDescription.text = config.Description;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            ClosePopup();
            
        }
    }
}
