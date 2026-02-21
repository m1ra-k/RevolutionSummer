using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Shared
public class GameProgression : MonoBehaviour
{
    [Header("DEBUG")]
    [SerializeField] private int debugConcertNumber;

    [Header("DATA")]
    public static GameProgression GameProgressionInstance;
    public SpriteCache SpriteCache; // move to GameData?
    public DialogueSystem DialogueSystemScript; // move to GameData?
    public FadeEffect FadeEffect;
    public string currentScene; // TODO: is this needed?

    [Header("UI")]
    public GameObject blackTransition;
    public GameObject cgTransition;
    public RectTransform blackTransitionRectTransform;

    [Header("LOGIC")]
    public bool transitioning = true;

    // BGM
    [SerializeField] private AudioSource audioSourceBGM;
    [SerializeField] private List<AudioClip> audioClipsBGM = new();
    private int currentBGM = -1;

    // SFX
    [SerializeField] private AudioSource audioSourceSFX;
    [SerializeField] private List<AudioClip> audioClipsSFX = new();
    
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        if (GameProgressionInstance == null)
        {
            GameProgressionInstance = this;

            DontDestroyOnLoad(gameObject);

            FadeEffect = GetComponent<FadeEffect>();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }

        audioSourceBGM = GetComponent<AudioSource>();
        audioSourceSFX = transform.GetChild(0).GetComponent<AudioSource>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(SceneSetup());
    }

    private IEnumerator SceneSetup()
    {
        GameData.fadeCoroutine = null;

        switch (SceneManager.GetActiveScene().name)
        {
            case "StartScreen":
                StartCoroutine(PlayBGM(0));
                break;
            case "VisualNovel":
                StartCoroutine(PlayBGM(1));
                break;
            case "Concert":
                switch (GameData.concertNumber)
                {
                    case 0:
                        StartCoroutine(PlayBGM(2));
                        break;
                    case 1:
                        StartCoroutine(PlayBGM(3));
                        break;
                    case 2:
                        StartCoroutine(PlayBGM(4));
                        break;
                }
                break;
            case "EndScreen":
                StartCoroutine(PlayBGM(5));
                break;
        }

        yield return new WaitForSeconds(0.25f);
    }

    public void SceneTransition(string scene)
    {
        transitioning = true;

        FadeEffect.FadeIn(blackTransition, fadeTime: 2f, scene: scene);
    }

    // BGM
    public IEnumerator PlayBGM(int index, float waitTime = 0.75f, GameObject gameObjectToDeactivate = null, float gameWaitTime = 0f, float fadeSpeed = 0.25f)
    {
        print($"switching to music at index {index}");
        
        float startVolume = audioSourceBGM.volume;

        if (index == -1) fadeSpeed = 1.75f;
        if (index == -2) fadeSpeed = 5f;

        for (float t = 0; t < fadeSpeed; t += Time.deltaTime)
        {
            audioSourceBGM.volume = Mathf.Lerp(startVolume, 0, t / fadeSpeed);
            if (index == -2 && audioSourceBGM.volume <= 0.05f)
            {
                yield break;
            }
            yield return null;
        }

        audioSourceBGM.volume = 0;
        audioSourceBGM.Stop();

        yield return new WaitForSeconds(waitTime);

        if (gameObjectToDeactivate)
        {
            gameObjectToDeactivate.SetActive(false);
        }

        if (index != -1)
        {
            for (float t = 0; t < fadeSpeed; t += Time.deltaTime)
            {
                audioSourceBGM.volume = Mathf.Lerp(0, 1, t / fadeSpeed);
                yield return null;
            }
            audioSourceBGM.volume = 1f;

            if (index == 5 || index != currentBGM)
            {
                audioSourceBGM.clip = audioClipsBGM[index];

                currentBGM = index;
            }

            audioSourceBGM.Play();

            yield return new WaitForSeconds(gameWaitTime);
        }
    }

    // SFX
    public void PlaySFX(int index)
    {
        audioSourceSFX.PlayOneShot(audioClipsSFX[index]);
    }

    // Fade
    public void Fade(string type, bool cg = false, string cgName = "")
    {
        if (!string.IsNullOrEmpty(cgName))
        {
            print($"getting {cgName}");
            cgTransition.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Art/CG/{cgName}");
        }
        
        if (type.Equals("in")) FadeEffect.FadeIn(!cg ? blackTransition : cgTransition, 0.5f);
            else FadeEffect.FadeOut(!cg ? blackTransition : cgTransition, 0.5f);
    }

}
