using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public int maxBarWidth = 256;
    
    [SerializeField] private Image valueBar;
    [SerializeField] private Image backgroundBar;
    [SerializeField] private TextMeshProUGUI valueText;

    [SerializeField] private Health health;


    private void Awake()
    {
        valueBar.rectTransform.sizeDelta = new Vector2(maxBarWidth, valueBar.rectTransform.sizeDelta.y);
        backgroundBar.rectTransform.sizeDelta = new Vector2(maxBarWidth, backgroundBar.rectTransform.sizeDelta.y);
        health.HealthChanged.AddListener(UpdateHealth);
        UpdateHealth(health.CurrentHealth, health.MaxHealth);
    }

    private void UpdateHealth(float newHealth, float maxHealth)
    {
        valueBar.rectTransform.sizeDelta = new Vector2(maxBarWidth * newHealth / maxHealth, valueBar.rectTransform.sizeDelta.y);
        valueText.text = $"{newHealth} / {maxHealth}";
    }
}
