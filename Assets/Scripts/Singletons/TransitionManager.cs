using System;
using UnityEngine;
using System.Collections;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager _Instance { get; private set; }
    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(_Instance.gameObject);
        }
        _Instance = this;
        if (fadeInOnAwake)
            FadeIn();
    }

    [SerializeField] private bool fadeInOnAwake = true;

    [SerializeField] private Animator anim;

    public void FadeOut(Action action)
    {
        StartCoroutine(PlayAnimationThenDoAction("FadeOut", action));
    }

    public void FadeOut()
    {
        anim.CrossFade("FadeOut", 0, 0);
    }

    public void FadeIn(Action action)
    {
        StartCoroutine(PlayAnimationThenDoAction("FadeIn", action));
    }

    public void FadeIn()
    {
        anim.CrossFade("FadeIn", 0, 0);
    }

    private IEnumerator PlayAnimationThenDoAction(string animationName, Action action)
    {
        // Debug.Log("Playing: " + animationName);
        anim.CrossFade(animationName, 0, 0);

        yield return new WaitUntil(() => AnimationHelper.AnimatorIsPlayingClip(anim, animationName));
        yield return new WaitUntil(() => !AnimationHelper.AnimatorIsPlayingClip(anim, animationName));

        action();
    }
}
