using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private Animator anim;

    [SerializeField] private float riseSpeed;

    public void Set(string text, Color color, Vector3 spawnPos)
    {
        Set(text, color, spawnPos, 1);
    }

    public void Set(float number, Color color, Vector3 spawnPos)
    {
        Set(System.Math.Round(number, 2).ToString(), color, spawnPos, 1);
    }

    public void Set(string prefix, float number, Color color, Vector3 spawnPos)
    {
        Set(prefix + System.Math.Round(number, 2).ToString(), color, spawnPos, 1);
    }

    public void Set(string text, Color color, Vector3 spawnPos, float scale)
    {
        this.text.text = text;
        this.text.color = color;
        transform.position = spawnPos;
        transform.localScale = Vector3.one * scale;
        StartCoroutine(Lifetime());
    }

    private IEnumerator Lifetime()
    {
        anim.CrossFade("PopupTextLifetime", 0, 0);

        yield return new WaitUntil(() => AnimationHelper.AnimatorIsPlayingClip(anim, "PopupTextLifetime"));

        while (AnimationHelper.AnimatorIsPlayingClip(anim, "PopupTextLifetime"))
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.up, Time.deltaTime * riseSpeed);

            yield return null;
        }

        ObjectPooler.popupTextPool.Release(this);
    }
}
