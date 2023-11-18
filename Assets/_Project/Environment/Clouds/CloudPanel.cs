using System;
using UnityEngine;

public class CloudPanel : MonoBehaviour
{
    [Header("Layer 1")]
    [SerializeField] private float scrollSpeed1 = 1f;
    [SerializeField] private float resetPosition1 = -4000f;
    [SerializeField]private RectTransform cloudPanel1;
    private Vector2 _originalPosition1;

    private void Awake()
    {
        cloudPanel1 = GetComponent<RectTransform>();
        _originalPosition1 = cloudPanel1.anchoredPosition;
    }

    private void FixedUpdate()
    {
        cloudPanel1.anchoredPosition += Vector2.left * (scrollSpeed1 * Time.fixedDeltaTime);
        if (cloudPanel1.anchoredPosition.x <= resetPosition1 + _originalPosition1.x)
            cloudPanel1.anchoredPosition = _originalPosition1;
    }
}