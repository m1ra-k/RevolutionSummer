using System.Collections;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;
using System.Collections.Generic;

// Concert
public class ConcertGameplay : MonoBehaviour
{
    public int healthPoints = 350;
    [SerializeField] private GameObject notePrefab;
    private Transform canvasTransform;
    private ICollection<Note> notes;
    private List<float> noteTimings = new();
    private float cumulativeTime;
    private Dictionary<string, int[]> attackPatterns = new()
    {
        { "square", new int[] { 0, 1, 2, 3 } },
        { "reverseSquare", new int[] { 3, 2, 1, 0 } },
        { "cross", new int[] { 0, 2, 1, 3 } },
        { "doubleDiagonalFromRight", new int[] { 0, 0, 2, 2 } },
        { "doubleDiagonalFromLeft", new int[] { 1, 1, 3, 3 } },
        { "upDown", new int[] { 1, 2, 0, 3 } },
        { "downUp", new int[] { 2, 1, 3, 0 } },
    };
    private int attackPatternsIndex;

    void Awake()
    {
        GameData.concertGameplay = this;

        canvasTransform = GameObject.Find("Canvas").transform;
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

        StartCoroutine(SpawnNotes());
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

                    // phase 2 and 10
                    if ((cumulativeTime > 20 && cumulativeTime <= 30) || (cumulativeTime > 88 && cumulativeTime <= 98))
                    {
                        DetermineAttackPatternSpawnLocation("upDown", noteBehavior);
                    }
                    // phase 3
                    else if (cumulativeTime > 30 && cumulativeTime <= 40)
                    {
                        DetermineAttackPatternSpawnLocation("square", noteBehavior);
                    }
                    // phase 4
                    else if (cumulativeTime > 40 && cumulativeTime <= 50)
                    {
                        DetermineAttackPatternSpawnLocation("downUp", noteBehavior);
                    }
                    // phase 5 and 8
                    else if ((cumulativeTime > 50 && cumulativeTime <= 55) || (cumulativeTime > 78 && cumulativeTime <= 85))
                    {
                        DetermineAttackPatternSpawnLocation("cross", noteBehavior);
                    }
                    // phase 7
                    else if (cumulativeTime > 40 && cumulativeTime <= 50)
                    {
                        DetermineAttackPatternSpawnLocation("doubleDiagonalFromRight", noteBehavior);
                    }
                    // phase 11
                    else if (cumulativeTime > 97 && cumulativeTime <= 107)
                    {
                        DetermineAttackPatternSpawnLocation("doubleDiagonalFromLeft", noteBehavior);
                    }
                    // phase 1
                    // phase 6
                    // phase 9
                    // phase 12
                    else
                    {
                        print("arbitrary");
                    }
                }
                break;
            case 1:
                break;
            case 2:
                break;
        }
    }

    private void DetermineAttackPatternSpawnLocation(string attackPaternName, NoteBehavior noteBehavior)
    {
        print($"{attackPaternName}");
        // print($"{attackPaternName} {attackPatterns[attackPaternName][attackPatternsIndex]}");
        noteBehavior.overrideSpawnPosition = attackPatterns[attackPaternName][attackPatternsIndex];
        attackPatternsIndex = (attackPatternsIndex < attackPatterns[attackPaternName].Length - 1) 
            ? ++attackPatternsIndex
            : 0;
    }
}