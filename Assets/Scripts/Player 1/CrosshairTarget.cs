using UnityEngine;
using UnityEngine.UI;

public class CrosshairTarget : MonoBehaviour
{
    public Camera playerCamera;
    public Image crosshair;

    public float distance = 50f;

    public Color normalColor = Color.white;
    public Color enemyColor = Color.red;
    public Color interactColor = new Color(1f, 0.5f, 0f); 

    public LayerMask interactLayer;

    void Update()
    {
        
        if (UIManager.Instance != null && UIManager.Instance.IsAnyUIOpen)
        {
            if (crosshair.enabled)
                crosshair.enabled = false;

            return;
        }
        else
        {
            if (!crosshair.enabled)
                crosshair.enabled = true;
        }


        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                crosshair.color = enemyColor;
                return;
            }

            if ((interactLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
            {
                crosshair.color = interactColor;
                return;
            }
        }

        crosshair.color = normalColor;
    }
}