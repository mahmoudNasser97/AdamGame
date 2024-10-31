using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CoinReward : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private TextMeshProUGUI counter;
    [SerializeField] private RectTransform targetTransform;
    [SerializeField] private Transform coinSpawnPoint;
    [SerializeField] CoinsManager coinManager;

    [Header("Animation Options")]
    [SerializeField] private AnimationType animationType = AnimationType.Bounce;
    [SerializeField] private float bounceIntensity = 1.5f;
    [SerializeField] private float scaleMultiplier = 1.2f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float moveDuration = 0.8f;

    private int coinsAmount;

    public enum AnimationType
    {
        Bounce,
        Scale,
        Fade,
        Rotate
    }

    public void AdjustCoinsAmount(int newCoinsAmount)
    {
        coinsAmount = newCoinsAmount;
        if (coinManager.Coins >= 0)
        {
            coinManager.GetCurrencyUpdate();
            for (int i = 0; i < coinsAmount; i++)
            {
                GameObject coin = Instantiate(coinPrefab, coinSpawnPoint.position, Quaternion.identity, transform);
                RectTransform coinRect = coin.GetComponent<RectTransform>();
                coinRect.anchoredPosition = ((RectTransform)coinSpawnPoint).anchoredPosition;

                AnimateCoin(coin, i * 0.1f);
            }
        }
    }

    private void AnimateCoin(GameObject coin, float delay)
    {
        RectTransform coinRect = coin.GetComponent<RectTransform>();
        switch (animationType)
        {
            case AnimationType.Bounce:
                coin.transform.DOScale(scaleMultiplier, 0.3f).SetDelay(delay).SetEase(Ease.OutBounce);
                break;
            case AnimationType.Scale:
                coin.transform.DOScale(1f, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);
                break;
            case AnimationType.Fade:
                coin.GetComponent<CanvasGroup>().DOFade(1f, fadeDuration).SetDelay(delay);
                break;
            case AnimationType.Rotate:
                coin.transform.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360).SetDelay(delay).SetEase(Ease.OutBack);
                break;
        }

        coinRect.DOAnchorPos(targetTransform.anchoredPosition, moveDuration)
            .SetDelay(delay + 0.5f).SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                coinRect.DOScale(1.1f, 0.2f).SetEase(Ease.OutBack)
                    .OnComplete(() => coinRect.DOScale(1f, 0.2f).SetEase(Ease.OutBack));
            });
        coin.transform.DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.5f).SetEase(Ease.Flash);
        coin.transform.DOScale(0f, 0.3f).SetDelay(delay + 1.5f).SetEase(Ease.OutBack)
            .OnComplete(() => Destroy(coin));
    }

    private IEnumerator CountDollars()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        PlayerPrefs.SetInt("CountDollar", PlayerPrefs.GetInt("CountDollar") + 50 + PlayerPrefs.GetInt("BPrize"));
        counter.text = PlayerPrefs.GetInt("CountDollar").ToString();
        PlayerPrefs.SetInt("BPrize", 0);
    }
}
