using UnityEngine;

// Concert
public class HealthBarBehavior : MonoBehaviour
{    
    private RectTransform rectTransform;
    private Vector2 currentSize;
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        UpdateWidth();
    }

    private void UpdateWidth()
    {
        currentSize = rectTransform.sizeDelta;
        rectTransform.sizeDelta = new Vector2(GameData.concertGameplay.healthPoints, currentSize.y);
    }
}
