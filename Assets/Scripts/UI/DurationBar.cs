using System.Collections;
using UnityEngine;

public class DurationBar : Bar
{
    public void Set(float duration)
    {
        StartCoroutine(Sequence(duration));
    }

    private new void Update()
    {
        if (fill.fillAmount > 0 && fill.fillAmount < 1)
        {
            fill.gameObject.SetActive(true);
        }
        else
        {
            fill.gameObject.SetActive(false);
        }
        base.Update();
    }

    private IEnumerator Sequence(float duration)
    {
        HardSetBar(0);
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            HardSetBar(t / duration);
            yield return null;
        }
    }
}
