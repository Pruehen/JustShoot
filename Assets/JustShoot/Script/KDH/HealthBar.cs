using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    PlayerCombatData data;

    Slider slider;
    public Gradient gradient;
    public Image fill;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        data = Player.Instance.GetCombatData();
        SetMaxHP(100);
    }

    void Update()
    {
        data = Player.Instance.GetCombatData();
        SetHP(data.playerCurHp);
    }
    public void SetMaxHP(float health)
    {
        slider.maxValue = health;
        slider.value = health;

        gradient.Evaluate(1f);
    }

    public void SetHP(float health)
    {
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
