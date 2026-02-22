using UnityEngine;

// Concert
public class NoteBehavior : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject landingPrefab;

    [Header("Distance Settings")]
    public float startDistance = 500f;
    private float targetRadius = 130f;
    private float triggerGap = 130f;
    
    [Header("Movement")]
    public float timeToReachTarget = 1.2f;
    
    private Vector2 moveDirection;
    private float moveSpeed;
    private bool reachedTarget = false;
    private bool hasSpawnedLandingPoint = false;
    private RectTransform rect;
    private GameObject marker;

    public int overrideSpawnPosition = -1;

    void Start()
    {
        rect = GetComponent<RectTransform>();

        Vector2[] corners = new Vector2[]
        {
            new Vector2(1, 1).normalized,
            new Vector2(-1, 1).normalized,
            new Vector2(-1, -1).normalized,
            new Vector2(1, -1).normalized
        };

        Vector2 randomDir = overrideSpawnPosition == -1
            ? corners[Random.Range(0, corners.Length)]
            : corners[overrideSpawnPosition];

        rect.anchoredPosition = randomDir * startDistance;
        
        moveDirection = -randomDir; 

        float travelDistance = startDistance - targetRadius;
        moveSpeed = travelDistance / timeToReachTarget;
    }

    void Update()
    {
        rect.anchoredPosition += moveDirection * moveSpeed * Time.deltaTime;

        float currentDist = rect.anchoredPosition.magnitude;

        if (!hasSpawnedLandingPoint && currentDist <= (targetRadius + triggerGap))
        {
            SpawnLandingPoint();
            hasSpawnedLandingPoint = true;
        }

        if (!reachedTarget && currentDist <= targetRadius - 30)
        {
            reachedTarget = true;
            OnNoteHitTarget();
        }
    }

    void SpawnLandingPoint()
    {
        Vector2 landingPos = (-moveDirection) * targetRadius;

        marker = Instantiate(landingPrefab, transform.parent);

        marker.transform.localScale *= 0.5f;
        
        RectTransform markerRect = marker.GetComponent<RectTransform>();
        
        markerRect.anchoredPosition = landingPos;
    }

    private void OnNoteHitTarget()
    {
        GameProgression.GameProgressionInstance.PlaySFX(1);
        GameData.concertGameplay.healthPoints -= 10;
        Destroy(gameObject);
    }

    public void OnDestroy()
    {
        Destroy(marker);
    }
}