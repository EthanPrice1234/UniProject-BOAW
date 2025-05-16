using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;

    private void Start()
    {
        healthSlider = GameObject.Find("Health Bar").GetComponent<Slider>();    
    }

    public void SetMaxHealth(int maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
    }

    public void SetCurrentHealth(int currentHealth)
    {
        healthSlider.value = currentHealth;
    }
}
