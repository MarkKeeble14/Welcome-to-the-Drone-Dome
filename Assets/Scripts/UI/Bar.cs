using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    [SerializeField] private float changeSpeed = 3f;
    private float targetPercentage = 1;
    [SerializeField] private Image fill;

    public void SetBar(float percentage)
    {
        targetPercentage = percentage;
    }

    public void HardSetBar(float percentage)
    {
        targetPercentage = percentage;
        fill.fillAmount = percentage;
    }

    private void ChangeFillAmount(float fillAmount)
    {
        fill.fillAmount = Mathf.Lerp(fill.fillAmount, fillAmount, Time.deltaTime * changeSpeed);
    }

    private void Update()
    {
        ChangeFillAmount(targetPercentage);
    }
}
