using UnityEngine;

public class VisualNovelGameplay : MonoBehaviour
{
    private TextAsset visualNovelJSONFile;

    void Awake()
    {
        visualNovelJSONFile = Resources.Load<TextAsset>($"Story/visual_novel_{GameData.concertNumber}");
    }

    void Start()
    {
        GameProgression.GameProgressionInstance.ShowDialogue(visualNovelJSONFile);
    }
}
