using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    [SerializeField] private float changeSpeed = 3f;
    private float targetPercentage;
    [SerializeField] private float startPercentage = 1;
    [SerializeField] protected Image fill;

    private void Start()
    {
        targetPercentage = startPercentage;
    }

    public void SetBar(float percentage)
    {
        targetPercentage = percentage;
    }

    public void HardSetBar(float percentage)
    {
        targetPercentage = percentage;
        fill.fillAmount = percentage;
    }

    protected virtual void ChangeFillAmount(float fillAmount)
    {
        fill.fillAmount = Mathf.MoveTowards(fill.fillAmount, fillAmount, Time.deltaTime * changeSpeed);
    }

    protected virtual void Update()
    {
        ChangeFillAmount(targetPercentage);
    }
}
