using UnityEngine;

public class SkillTreeHintUI : MonoBehaviour
{
    void Update()
    {
        if (UIManager.Instance != null)
        {
            gameObject.SetActive(!UIManager.Instance.IsAnyUIOpen);
        }
    }
}