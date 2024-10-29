using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinsUIHandler : MonoBehaviour
{
    public CoinsManager currencyManager;
    public TMP_Text coinsText;

    private void Start()
    {
        currencyManager.OnCurrencyChanged += UpdateCurrencyUI;

        UpdateCurrencyUI();
    }
    public void AddCoinsFromUI(int amount)
    {
        currencyManager.AddCoins(amount);
    }
    public void SubtractCoinsFromUI(int amount)
    {
        currencyManager.SubtractCoins(amount);
    }
    public void UpdateCurrencyUI()
    {
        coinsText.text = currencyManager.Coins.ToString();
    }
    private void OnDestroy()
    {
        currencyManager.OnCurrencyChanged -= UpdateCurrencyUI;
    }
}
