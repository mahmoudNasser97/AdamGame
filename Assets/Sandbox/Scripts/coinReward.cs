using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CoinReward : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;                  // Reference to a single coin prefab
    [SerializeField] private TextMeshProUGUI counter;                // UI counter to display total coins
    [SerializeField] private RectTransform targetTransform;          // Target RectTransform (UI Position)
    [SerializeField] private Transform coinSpawnPoint;               // Optional spawn point for coins

    [Header("Animation Options")]
    [SerializeField] private AnimationType animationType = AnimationType.Bounce; // Animation type option
    [SerializeField] private float bounceIntensity = 1.5f;            // Bounce intensity for animations
    [SerializeField] private float scaleMultiplier = 1.2f;            // Scale multiplier for scaling effects
    [SerializeField] private float fadeDuration = 0.5f;               // Duration for fade animations
    [SerializeField] private float moveDuration = 0.8f;               // Duration for the move animation

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

        for (int i = 0; i < coinsAmount; i++)
        {
            GameObject coin = Instantiate(coinPrefab, coinSpawnPoint.position, Quaternion.identity, transform);
            RectTransform coinRect = coin.GetComponent<RectTransform>();
            coinRect.anchoredPosition = ((RectTransform)coinSpawnPoint).anchoredPosition;

            AnimateCoin(coin, i * 0.1f);                          // Animate with a slight delay for each coin
        }
    }

    private void AnimateCoin(GameObject coin, float delay)
    {
        RectTransform coinRect = coin.GetComponent<RectTransform>();

        // Scale up animation
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

        // Move to target position with ease and delay
        coinRect.DOAnchorPos(targetTransform.anchoredPosition, moveDuration)
            .SetDelay(delay + 0.5f).SetEase(Ease.OutQuad) // Smooth easing for movement
            .OnComplete(() =>
            {
                // Add a slight bounce effect at the end position
                coinRect.DOScale(1.1f, 0.2f).SetEase(Ease.OutBack)
                    .OnComplete(() => coinRect.DOScale(1f, 0.2f).SetEase(Ease.OutBack));
            });

        // Rotate and scale down animations
        coin.transform.DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.5f).SetEase(Ease.Flash);
        coin.transform.DOScale(0f, 0.3f).SetDelay(delay + 1.5f).SetEase(Ease.OutBack)
            .OnComplete(() => Destroy(coin));                     // Destroy the coin after animation
    }

    private IEnumerator CountDollars()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        PlayerPrefs.SetInt("CountDollar", PlayerPrefs.GetInt("CountDollar") + 50 + PlayerPrefs.GetInt("BPrize"));
        counter.text = PlayerPrefs.GetInt("CountDollar").ToString();
        PlayerPrefs.SetInt("BPrize", 0);
    }
}
