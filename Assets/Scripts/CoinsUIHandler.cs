using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinsUIHandler : MonoBehaviour
{
    public CoinsManager currencyManager;
    public TMP_Text coinsText;
    public TMP_Text coinsSliderText;
    public Slider coinSlider;
    public int maxCoins = 100000;
    private int coinsAdded=500;
    private void Start()
    {
        currencyManager.OnCurrencyChanged += UpdateCurrencyUI;
        currencyManager.OnCurrencyChanged += UpdateCoins;

        coinSlider.maxValue = maxCoins;
        coinSlider.value = currencyManager.Coins;
        coinsSliderText.text = currencyManager.Coins.ToString();
        UpdateCurrencyUI();
    }
    public void AddCoinsFromUI(int amount)
    {
        currencyManager.AddCoins(amount);
    }
    public void SubtractCoinsFromUI(int amount)
    {
        currencyManager.SubtractCoins(amount);
        //coinsSliderText.text += amount;
        UpdateCoins();
    }
    public void UpdateCurrencyUI()
    {
        coinsText.text = currencyManager.Coins.ToString();
        
    }
    private void OnDestroy()
    {
        currencyManager.OnCurrencyChanged -= UpdateCurrencyUI;
    }

    public void UpdateCoins()
    {
        int currentCoins = int.Parse(coinsSliderText.text);
        currentCoins += coinsAdded;
        coinsSliderText.text = currentCoins.ToString();
        coinSlider.value = int.Parse(coinsSliderText.text);
    }
}
