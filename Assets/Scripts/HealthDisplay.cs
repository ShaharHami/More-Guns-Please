using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    public Canvas healthDisplayCanvas;
    public TextMeshProUGUI healthDisplay;
    public Image healthBar;
    private bool rotateWithParent;
    private int baseHealth;
    private Vector3 healthBarScale;

    private void Awake()
    {
        healthBarScale = new Vector3(1, 1, 1);
        if (healthBar != null)
        {
            healthBar.transform.localScale = healthBarScale;
        }
    }

    public void SetHealth(int health, bool rotate)
    {
        baseHealth = health;
        ChangeHealth(health);
        rotateWithParent = rotate;
    }

    public void ChangeHealth(int health)
    {
        float currentHealthPercentage = (float) health / (float) baseHealth;
        healthDisplay.text = health.ToString();
        if (healthBar != null)
        {
            healthBarScale.x = currentHealthPercentage;
            healthBar.transform.localScale = healthBarScale;
        }
    }

    private void Update()
    {
        if (rotateWithParent)
        {
            healthDisplayCanvas.transform.rotation = Quaternion.Euler(90, 0, 0);
            if (healthBar != null)
            {
                healthBar.transform.rotation = Quaternion.Euler(90, 0, 0);
            }
        }
    }
}