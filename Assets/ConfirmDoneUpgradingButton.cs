using UnityEngine;
using UnityEngine.UI;

public class ConfirmDoneUpgradingButton : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        button.onClick.AddListener(delegate { ConfirmDoneUpgrading(); });
    }

    private void ConfirmDoneUpgrading()
    {
        FindObjectOfType<ArenaManager>().ConfirmDoneUpgrading();
    }
}

