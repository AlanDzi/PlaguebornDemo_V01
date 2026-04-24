using UnityEngine;
using UnityEngine.UI;

public class SkillTreeHintUI : MonoBehaviour
{
    CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (UIManager.Instance == null) return;

        bool show = !UIManager.Instance.IsAnyUIOpen;

        canvasGroup.alpha = show ? 1f : 0f;
        canvasGroup.interactable = show;
        canvasGroup.blocksRaycasts = show;
    }
}