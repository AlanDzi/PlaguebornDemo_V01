using System;
using UnityEngine;
using UnityEngine.UI;

using TMPro;


public class ComicPanelUI : MonoBehaviour
{
    public static ComicPanelUI Instance;

    public GameObject panel;
    public Image comicImage;
    public TextMeshProUGUI continueText;
    public TextMeshProUGUI comicDescription;

    private Action onClose;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    void Update()
    {
        if (!panel.activeSelf)
            return;

        float alpha = Mathf.PingPong(Time.unscaledTime * 2f, 1f);

        Color c = continueText.color;
        c.a = alpha;
        continueText.color = c;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Hide();
        }
    }

    public void Show(
    Sprite sprite,
    string text,
    Action callback = null)
    {
        panel.SetActive(true);
        panel.transform.SetAsLastSibling();
       
        comicImage.sprite = sprite;

        comicDescription.text = text;

        onClose = callback;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;

        PlayerController player =
            FindFirstObjectByType<PlayerController>();

        if (player != null)
            player.enabled = false;

        WeaponController weapon =
            FindFirstObjectByType<WeaponController>();

        if (weapon != null)
            weapon.enabled = false;
    }

    public void Hide()
    {
        panel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;

        onClose?.Invoke();

        PlayerController player =
    FindFirstObjectByType<PlayerController>();

        if (player != null)
            player.enabled = true;

        WeaponController weapon =
            FindFirstObjectByType<WeaponController>();

        if (weapon != null)
            weapon.enabled = true;

        onClose?.Invoke();
    }
}