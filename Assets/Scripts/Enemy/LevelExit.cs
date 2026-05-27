using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [Header("Boss do pokonania")]
    public GameObject boss;

    [Header("Ustawienia")]
    public KeyCode interactKey = KeyCode.E;
    public float interactDistance = 3f;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null || boss == null)
            return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactDistance && Input.GetKeyDown(interactKey))
        {
            // jeœli boss zosta³ zniszczony
            if (boss == null)
            {
                LoadNextLevel();
            }
        }
    }

    void LoadNextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex + 1);
    }
}