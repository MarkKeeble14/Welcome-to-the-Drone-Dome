using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmDoneShopVisitButton : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        button.onClick.AddListener(delegate { ConfirmDoneShopVisit(); });
    }

    private void ConfirmDoneShopVisit()
    {
        FindObjectOfType<ArenaManager>().ConfirmDoneShopVisit();
    }
}

