using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradePointsAvailableDisplay : MonoBehaviour
{
    private UpgradeManager cachedInstance;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private string prefix = "Remaining: ";
    [SerializeField] private string suffix = "";

    private void Start()
    {
        cachedInstance = UpgradeManager._Instance;
    }

    // Update is called once per frame
    void Update()
    {
        text.text = prefix + cachedInstance.UpgradePointsAvailable.ToString() + suffix;
    }
}
