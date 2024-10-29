using UnityEngine;

public class CoinSpawnerDOTween : MonoBehaviour
{
    public GameObject coinPrefab;
    public RectTransform targetPosition;
    public int numberOfCoins = 5;

    public void SpawnCoins()
    {
        for (int i = 0; i < numberOfCoins; i++)
        {
            // Instantiate the coin prefab and set it as a child of the spawner's parent (usually the Canvas)
            GameObject coin = Instantiate(coinPrefab, transform.parent);
            RectTransform coinRectTransform = coin.GetComponent<RectTransform>();

            // Set the starting position relative to the spawner's position
            coinRectTransform.anchoredPosition = ((RectTransform)transform).anchoredPosition;
            coinRectTransform.localScale = Vector3.one; // Ensure scale is consistent
            
            // Initialize the DOTween animation with the target position
            CoinAnimationDOTween animation = coin.GetComponent<CoinAnimationDOTween>();
            animation.targetPosition = targetPosition;
            animation.StartCoinAnimation();
        }
    }
}
