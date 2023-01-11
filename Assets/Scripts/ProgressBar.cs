using System;
using TMPro;
using UnityEngine;

public class ProgressBar : Bar
{
    public Action OnFill;
    private bool hasCalled;

    public void ResetHasCalled()
    {
        hasCalled = false;
    }

    protected override void ChangeFillAmount(float fillAmount)
    {
        base.ChangeFillAmount(fillAmount);
        if (!hasCalled && fill.fillAmount == 1)
        {
            OnFill();
            hasCalled = true;
        }
    }
}
