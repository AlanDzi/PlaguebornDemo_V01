using TMPro;
using UnityEngine;

public class ItemTooltipUI : MonoBehaviour
{
    public static ItemTooltipUI Instance;

    public GameObject root;

    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI descriptionText;

    void Awake()
    {
        Instance = this;

        Hide();
    }

    public void Show(ItemData item)
    {
        if (item == null)
            return;

        root.SetActive(true);

        itemNameText.text = item.itemName;

        string desc = item.description;

        // Jeœli broñ - poka¿ statystyki
        if (item.weaponData != null)
        {
            desc += "\n\n";

            desc += "Damage: " +
                item.weaponData.baseDamage + "\n";

            desc += "Attack Speed: " +
                item.weaponData.attackSpeed + "\n";

            desc += "Range: " +
                item.weaponData.attackRange + "\n";

            desc += "Crit Chance: " +
                Mathf.RoundToInt(item.weaponData.critChance * 100f) + "%";
        }

        descriptionText.text = desc;
    }

    public void Hide()
    {
        root.SetActive(false);
    }
    void Update()
    {
        if (root.activeSelf)
        {
            transform.position =
    Input.mousePosition + new Vector3(100, 100);
        }
    }
}