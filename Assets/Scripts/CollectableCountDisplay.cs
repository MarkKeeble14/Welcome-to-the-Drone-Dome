using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectableCountDisplay : MonoBehaviour
{
    private GameManager cachedInstance;
    [SerializeField] private TextMeshProUGUI text;
    private void Start()
    {
        cachedInstance = GameManager._Instance;
    }

    // Update is called once per frame
    void Update()
    {
        text.text = cachedInstance.CurrentPlayerResource.ToString();
    }
}
