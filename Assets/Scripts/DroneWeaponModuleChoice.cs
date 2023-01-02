using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DroneWeaponModuleChoice : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI actualCostText;
    private int actualCost;
    [SerializeField] private TextMeshProUGUI intendedCostText;
    private int intendedCost;
    [SerializeField] private TextMeshProUGUI categoryText;
    public void Set(ModuleType type, int defaultCost)
    {
        nameText.text = type.ToString();
        intendedCost = defaultCost;
        categoryText.text = GameManager._Instance.GetModuleCategory(type).ToString();

        Button b = GetComponentInChildren<Button>();
        b.onClick.AddListener(delegate
        {
            if (ShopManager._Instance.PurchaseWeaponModule(type, actualCost))
            {
                Debug.Log("Successfully Purchased: " + type);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Failed to Purchase: " + type);
            }
        });
    }

    private void Update()
    {
        if (ShopManager._Instance.FreePurchase)
        {
            actualCost = 0;
            intendedCostText.gameObject.SetActive(true);
            intendedCostText.text = intendedCost.ToString();
            intendedCostText.fontStyle = FontStyles.Strikethrough;

            actualCostText.text = "Free";
        }
        else
        {
            intendedCostText.fontStyle = FontStyles.Normal;
            intendedCostText.gameObject.SetActive(false);

            actualCost = intendedCost;
            actualCostText.text = actualCost.ToString();
        }
    }
}
