using UnityEngine;

public class AmbientMusic : MonoBehaviour
{
    public static AmbientMusic Instance;

    public AudioSource source;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (source == null)
            source = GetComponent<AudioSource>();

        if (source != null && !source.isPlaying)
            source.Play();
    }
}
