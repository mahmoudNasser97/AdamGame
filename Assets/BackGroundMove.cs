using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BackGroundMove : MonoBehaviour
{
    public float moveDistance = 10f;
    public float duration = 5f;
    public Ease easeEffect;

    void Update()
    {
        // Move left and right continuously
        transform.DOMoveX(transform.position.x - moveDistance, duration)
            .SetEase(easeEffect);
    }
}
