using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CoinAnimationDOTween : MonoBehaviour
{
    public RectTransform targetPosition; // Target UI element for coin collection
    public float animationDuration = 1f; // Duration of the animation

    private RectTransform coinTransform;

    private void Awake()
    {
        coinTransform = GetComponent<RectTransform>();
    }

    public void StartCoinAnimation()
    {
        // Start the animation using DOTween
        coinTransform.DOAnchorPos(targetPosition.anchoredPosition, animationDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // Optional: Add effects or increase coin counter here
                Destroy(gameObject); // Destroy coin after reaching the target
            });
    }
}
