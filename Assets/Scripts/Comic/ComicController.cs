using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ComicController : MonoBehaviour
{
    [Header("UI")]
    public Image comicImage;
    public Image fadePanel;

    [Header("Panels")]
    public Sprite[] panels;

    [Header("Narrator")]
    public AudioSource narratorSource;
    public AudioClip[] panelClips;

    [Header("Background Sound")]
    public AudioSource backgroundSource;
    public AudioClip rainClip;

    [Header("Fade")]
    public float fadeTime = 0.4f;

    private int index = 0;
    private bool isTransitioning = false;

    void Start()
    {
        
        if (narratorSource == null)
            narratorSource = GetComponent<AudioSource>();

        if (backgroundSource == null)
            backgroundSource = gameObject.AddComponent<AudioSource>();

        
        backgroundSource.loop = true;
        backgroundSource.playOnAwake = false;
        backgroundSource.clip = rainClip;

        if (rainClip != null)
            backgroundSource.Play();

        comicImage.sprite = panels[0];

        PlayNarrator(0);

        SetAlpha(fadePanel, 1);
        StartCoroutine(FadeIn());
    }

    void Update()
    {
        if (isTransitioning) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(NextPanel());
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Dungeon");
        }
    }

    IEnumerator NextPanel()
    {
        isTransitioning = true;

        yield return FadeOut();

        index++;

        if (index < panels.Length)
        {
            comicImage.sprite = panels[index];

            PlayNarrator(index);

            yield return FadeIn();
        }
        else
        {
            StopBackground();
            SceneManager.LoadScene("Dungeon");
        }

        isTransitioning = false;
    }

    // ================= NARRATOR =================

    void PlayNarrator(int panelIndex)
    {
        if (narratorSource == null) return;

        if (panelClips == null || panelClips.Length == 0) return;

        if (panelIndex < 0 || panelIndex >= panelClips.Length) return;

        AudioClip clip = panelClips[panelIndex];

        if (clip == null) return;

        narratorSource.Stop();
        narratorSource.clip = clip;
        narratorSource.Play();
    }

    // ================= BACKGROUND =================

    void StopBackground()
    {
        if (backgroundSource != null)
            backgroundSource.Stop();
    }

    // ================= FADE =================

    IEnumerator FadeOut()
    {
        float t = 0;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            SetAlpha(fadePanel, t / fadeTime);
            yield return null;
        }

        SetAlpha(fadePanel, 1);
    }

    IEnumerator FadeIn()
    {
        float t = fadeTime;

        while (t > 0)
        {
            t -= Time.deltaTime;
            SetAlpha(fadePanel, t / fadeTime);
            yield return null;
        }

        SetAlpha(fadePanel, 0);
    }

    void SetAlpha(Image img, float a)
    {
        Color c = img.color;
        c.a = a;
        img.color = c;
    }
}
