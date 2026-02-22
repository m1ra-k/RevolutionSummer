using System.Collections;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;
using System.Collections.Generic;
using TMPro;

// Concert
public class ConcertGameplay : MonoBehaviour
{
    public int healthPoints = 700;
    [SerializeField] private GameObject notePrefab;
    private Transform canvasTransform;
    private ICollection<Note> notes;
    private List<float> noteTimings = new();
    private float cumulativeTime;
    private Dictionary<string, int[]> attackPatterns = new()
    {
        { "s0p0", new int[] { 1, 0, 2, 3 } },
        { "s0p1", new int[] { 0, 1, 2, 3 } },
        { "s0p2", new int[] { 1, 1, 0, 0, 2, 2, 3, 3 } },
        { "s0p3", new int[] { 1, 3, 0, 2 } },
        { "s0p4", new int[] { 1, 1, 0, 2, 2, 3 } },
        { "s0p5", new int[] { 1, 1, 0, 0 } },
        { "s0p6", new int[] { 0, 1, 2, 1, 0, 3 } },
        { "s1p0", new int[] { 0, 1, 2, 3 } },
        { "s1p1", new int[] { 0, 2, 1, 3 } },
        { "s1p2", new int[] { 1, 0, 3, 2 } },
        { "s1p3", new int[] { 1, 1, 0, 0 } },
        { "s1p4", new int[] { 0, 1, 2 } },
        { "s1p5", new int[] { 0, 1, 2, 2, 3 } },
        { "s2p0", new int[] { 0, 1, 2, 3 } },
        { "s2p1", new int[] { 1, 3, 2, 0 } },
        { "s2p2", new int[] { 3, 2, 1, 0 } },
        { "s2p3", new int[] { 2, 3, 1, 0 } },
        { "s2p4", new int[] { 0, 2, 1, 3 } },
        { "s2p5", new int[] { 0, 3, 2, 1 } }
    };
    private int attackPatternsIndex;
    private Coroutine currentCoroutine;
    private bool finishedConcert;
    private bool endScene;
    private TextMeshProUGUI endSceneText;

    void Awake()
    {
        GameData.concertGameplay = this;

        canvasTransform = GameObject.Find("Canvas").transform;
        endSceneText = GameObject.Find("Canvas").transform.Find("EndSceneText").GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, $"Sequence{GameData.concertNumber}.mid");

        var midiFile = MidiFile.Read(filePath);
                
        notes = midiFile.GetNotes();
        // Debug.Log($"found {notes.Count} notes.");

        // StringBuilder sb = new StringBuilder();

        // sb.AppendLine("TIME (s)\tNOTE NUM");
        // sb.AppendLine("------------------------------------");

        foreach (var note in notes)
        {
            var timeSpan = note.TimeAs<MetricTimeSpan>(midiFile.GetTempoMap());
            float hitTime = timeSpan.TotalMicroseconds / 1000000f;
            
            // sb.AppendLine($"{hitTime:F2}s\t\t({note.NoteNumber})");
            noteTimings.Add(hitTime);
        }

        // Debug.Log(sb.ToString());

        currentCoroutine = StartCoroutine(SpawnNotes());
    }

    void Update()
    {
        if (!endScene)
        {
            if (!finishedConcert && healthPoints == 0)
            {
                endScene = true;
                StopCoroutine(currentCoroutine);
                currentCoroutine = StartCoroutine(EndScene(false));
            }
            else if (finishedConcert)
            {
                endScene = true;
                StopCoroutine(currentCoroutine);
                currentCoroutine = StartCoroutine(EndScene(true));
            }
        }
    }

    private IEnumerator SpawnNotes()
    {
        switch (GameData.concertNumber)
        {
            case 0:
                yield return new WaitForSeconds(1.6f);

                foreach (float hitTime in noteTimings)
                {
                    while (cumulativeTime < hitTime)
                    {
                        cumulativeTime += Time.deltaTime;
                        yield return null; 
                    }

                    GameObject note = Instantiate(notePrefab, canvasTransform);

                    NoteBehavior noteBehavior = note.GetComponent<NoteBehavior>();

                    // phase 1
                    if (cumulativeTime <= 20)
                    {
                        DetermineAttackPatternSpawnLocation("s0p0", noteBehavior);
                    }
                    // phase 2, 4, 5, 8
                    else if ((cumulativeTime > 20 && cumulativeTime <= 40) || (cumulativeTime > 54 && cumulativeTime <= 76) || (cumulativeTime > 76 && cumulativeTime <= 78) || (cumulativeTime > 107 && cumulativeTime <= 116))
                    {
                        DetermineAttackPatternSpawnLocation("s0p1", noteBehavior);
                    }
                    // phase 3
                    else if (cumulativeTime > 40 && cumulativeTime <= 54)
                    {
                        DetermineAttackPatternSpawnLocation("s0p2", noteBehavior);
                    }
                    // phase 6
                    else if (cumulativeTime > 78 && cumulativeTime <= 97)
                    {
                        DetermineAttackPatternSpawnLocation("s0p3", noteBehavior);
                    }
                    // phase 7
                    else if (cumulativeTime > 97 && cumulativeTime <= 107)
                    {
                        DetermineAttackPatternSpawnLocation("s0p4", noteBehavior);
                    }
                    // phase 9
                    else if (cumulativeTime > 116 && cumulativeTime <= 126)
                    {
                        DetermineAttackPatternSpawnLocation("s0p5", noteBehavior);
                    }
                    // phase 10
                    else if (cumulativeTime > 126)
                    {
                        DetermineAttackPatternSpawnLocation("s0p6", noteBehavior);
                    }
                }
                break;
            case 1:
                yield return new WaitForSeconds(1.72f);

                foreach (float hitTime in noteTimings)
                {
                    while (cumulativeTime < hitTime)
                    {
                        cumulativeTime += Time.deltaTime;
                        yield return null; 
                    }

                    GameObject note = Instantiate(notePrefab, canvasTransform);

                    NoteBehavior noteBehavior = note.GetComponent<NoteBehavior>();

                    // phase 1
                    if (cumulativeTime <= 14)
                    {
                        DetermineAttackPatternSpawnLocation("s1p0", noteBehavior);
                    }
                    // phase 2 and 4
                    else if ((cumulativeTime > 14 && cumulativeTime <= 24) || (cumulativeTime > 31 && cumulativeTime <= 82))
                    {
                        DetermineAttackPatternSpawnLocation("s1p1", noteBehavior);
                    }
                    // phase 3, 4, and 7
                    else if ((cumulativeTime > 24 && cumulativeTime <= 31) || (cumulativeTime > 82 && cumulativeTime <= 89) || (cumulativeTime > 113 && cumulativeTime <= 116))
                    {
                        DetermineAttackPatternSpawnLocation("s1p2", noteBehavior);
                    }
                    // phase 6 and 8
                    else if ((cumulativeTime > 89 && cumulativeTime <= 113) || (cumulativeTime > 116 && cumulativeTime <= 127))
                    {
                        DetermineAttackPatternSpawnLocation("s1p3", noteBehavior);
                    }
                    // phase 9
                    else if (cumulativeTime > 127 && cumulativeTime <= 130)
                    {
                        DetermineAttackPatternSpawnLocation("s1p4", noteBehavior);
                    }
                    // phase 10
                    else if (cumulativeTime > 130)
                    {
                        DetermineAttackPatternSpawnLocation("s1p5", noteBehavior);
                    }
                }
                break;
            case 2:
                yield return new WaitForSeconds(1.85f);

                foreach (float hitTime in noteTimings)
                {
                    while (cumulativeTime < hitTime)
                    {
                        cumulativeTime += Time.deltaTime;
                        yield return null; 
                    }

                    GameObject note = Instantiate(notePrefab, canvasTransform);

                    NoteBehavior noteBehavior = note.GetComponent<NoteBehavior>();

                    // phase 1, 4, and 8
                    if ((cumulativeTime <= 18) || (cumulativeTime > 43 && cumulativeTime <= 59) || (cumulativeTime > 92 && cumulativeTime <= 108))
                    {
                        DetermineAttackPatternSpawnLocation("s2p0", noteBehavior);
                    }
                    // phase 2 and 6
                    else if ((cumulativeTime > 18 && cumulativeTime <= 40) || (cumulativeTime > 67 && cumulativeTime <= 89))
                    {
                        DetermineAttackPatternSpawnLocation("s2p1", noteBehavior);
                    }
                    // phase 3 and 7
                    else if ((cumulativeTime > 40 && cumulativeTime <= 43) || (cumulativeTime > 89 && cumulativeTime <= 92))
                    {
                        DetermineAttackPatternSpawnLocation("s2p2", noteBehavior);
                    }
                    // phase 5
                    else if (cumulativeTime > 59 && cumulativeTime <= 67)
                    {
                        DetermineAttackPatternSpawnLocation("s2p3", noteBehavior);
                    }
                    // phase 9
                    else if (cumulativeTime > 108 && cumulativeTime <= 125)
                    {
                        DetermineAttackPatternSpawnLocation("s2p4", noteBehavior);
                    }
                    // phase 10
                    else if (cumulativeTime > 125)
                    {
                        DetermineAttackPatternSpawnLocation("s2p5", noteBehavior);
                    }
                }
                break;
        }

        yield return new WaitForSeconds(3f);

        if (healthPoints > 0) finishedConcert = true;
    }

    private IEnumerator EndScene(bool success)
    {
        endSceneText.text = success
            ? "SUCCESS!!"
            : "FAILURE!!";

        float duration = 1.5f;
        float currentTime = 0f;

        Color originalColor = endSceneText.color;
        endSceneText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            
            float alpha = Mathf.Lerp(0f, 1f, currentTime / duration);
            
            endSceneText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            
            yield return null;
        }

        endSceneText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

        yield return new WaitForSeconds(1.5f);

        if (success)
        {
            GameData.concertNumber++;
            GameProgression.GameProgressionInstance.SceneTransition("VisualNovel");
        }
        else
        {
            GameProgression.GameProgressionInstance.SceneTransition("Concert");
        }
    }

    private void DetermineAttackPatternSpawnLocation(string attackPaternName, NoteBehavior noteBehavior)
    {
        print($"{attackPaternName}");
        noteBehavior.overrideSpawnPosition = attackPatterns[attackPaternName][attackPatternsIndex];
        attackPatternsIndex = (attackPatternsIndex < attackPatterns[attackPaternName].Length - 1) 
            ? ++attackPatternsIndex
            : 0;
    }
}