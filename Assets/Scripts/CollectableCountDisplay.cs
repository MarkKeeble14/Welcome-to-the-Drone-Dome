using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectableCountDisplay : MonoBehaviour
{
    private ShopManager cachedInstance;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private string prefix = "$";
    private void Start()
    {
        cachedInstance = ShopManager._Instance;
    }

    // Update is called once per frame
    void Update()
    {
        text.text = prefix + cachedInstance.CurrentPlayerResource.ToString();
    }
}
