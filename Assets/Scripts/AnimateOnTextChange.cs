using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnimateOnTextChange : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private string animationName;
    [SerializeField] private TextMeshProUGUI text;
    private string lastString;

    // Update is called once per frame
    void Update()
    {
        // if text has changed
        if (text.text != lastString)
        {
            anim.CrossFade(animationName, 0, 0);
        }
        lastString = text.text;
    }
}
