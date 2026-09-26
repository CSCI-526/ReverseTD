using UnityEngine;
using TMPro;

public class GoldDisplay : MonoBehaviour
{
    [SerializeField] private OffenseData offenseData;
    [SerializeField] private TMP_Text goldText;

    private void Awake()
    {
        if (goldText == null)
            goldText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (offenseData != null && goldText != null)
        {
            goldText.text = $"$ {offenseData.gold:N0}";
        }
    }
}