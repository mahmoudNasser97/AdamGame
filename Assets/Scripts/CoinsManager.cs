using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsManager : MonoBehaviour
{
    private const string CoinsKey = "Coins";

    public int Coins { get; private set; }
    public event Action OnCurrencyChanged;

    private void Start()
    {
        LoadCurrency();
    }
    public void AddCoins(int amount)
    {
        Coins += amount;
        OnCurrencyChanged?.Invoke();
        SaveCurrency();
    }
    public void SubtractCoins(int amount)
    {
        if (Coins >= amount)
        {
            Coins -= amount;
            OnCurrencyChanged?.Invoke();
            SaveCurrency();
        }
    }
    public void SubtractAllCoins()
    {
        Coins -= Coins;
        OnCurrencyChanged?.Invoke();
        SaveCurrency();
    }
    public void SetCoins(int newAmount)
    {
        Coins = newAmount;
        OnCurrencyChanged?.Invoke();
        SaveCurrency();
    }
    private void SaveCurrency()
    {
        PlayerPrefs.SetInt(CoinsKey, Coins);
        PlayerPrefs.Save();
    }
    private void LoadCurrency()
    {
        Coins = PlayerPrefs.GetInt(CoinsKey, 0);
        OnCurrencyChanged?.Invoke();
    }
}
