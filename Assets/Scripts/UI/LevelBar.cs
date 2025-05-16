using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelBar : MonoBehaviour
{
    public Slider levelSlider;

    private void Start()
    {
        levelSlider = GameObject.Find("Level Bar").GetComponent<Slider>();
    }

    public void ResetLevelBar(int nextLevelXP)
    {
        levelSlider.maxValue = nextLevelXP;
        levelSlider.value = 0;
    }

    public void SetCurrentXP(int currentXP)
    {
        levelSlider.value = currentXP;
    }
}
