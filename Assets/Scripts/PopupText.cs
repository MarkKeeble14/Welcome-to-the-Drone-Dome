using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private Animator anim;

    [SerializeField] private float riseSpeed;

    public void Set(string text, Color color, float spawnHeight)
    {
        Set(text, color, spawnHeight, 1);
    }

    public void Set(float number, Color color, float spawnHeight)
    {
        Set(System.Math.Round(number, 2).ToString(), color, spawnHeight, 1);
    }

    public void Set(string prefix, float number, Color color, float spawnHeight)
    {
        Set(prefix + System.Math.Round(number, 2).ToString(), color, spawnHeight, 1);
    }

    public void Set(string text, Color color, float spawnHeight, float scale)
    {
        this.text.text = text;
        this.text.color = color;
        transform.position += Vector3.up * spawnHeight;
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

        Destroy(gameObject);
    }
}
