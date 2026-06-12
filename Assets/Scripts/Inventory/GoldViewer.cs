using TMPro;
using UnityEngine;

public class GoldViewer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI goldText;

    void OnEnable()
    {
        UpdateGold();

        if (GoldManager.Instance != null)
        {
            UpdateGold();
        }
    }

    void Update()
    {
        UpdateGold();
    }

    void UpdateGold()
    {
        if (goldText == null)
            return;

        if (GoldManager.Instance == null)
            return;

        goldText.text =
            GoldManager.Instance.Gold.ToString();
    }
}