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

    [SerializeField] private Button discard;
    [SerializeField] private Button purchase;


    public void Set(ModuleType type, int defaultCost, int indexInList)
    {
        nameText.text = EnumToStringHelper.GetStringValue(type);
        intendedCost = defaultCost;
        categoryText.text = GameManager._Instance.GetModuleCategory(type).ToString();

        discard.onClick.AddListener(delegate
        {
            ShopManager._Instance.RemoveModuleAtPosition(indexInList);
        });

        purchase.onClick.AddListener(delegate
        {
            if (ShopManager._Instance.PurchaseWeaponModule(type, actualCost, indexInList))
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
