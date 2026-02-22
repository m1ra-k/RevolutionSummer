using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

// Concert
public class GuitarHeartBehavior : MonoBehaviour
{
    public float range = 500f;
    public float rayDuration = 0.5f;
    public int rayCount = 25;
    public float totalAngle = 20f;
    private Coroutine performShoot;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (performShoot != null)
            {
                StopCoroutine(performShoot);
            }
            performShoot = StartCoroutine(PerformShoot());
        }
        Rotate();
    }

    IEnumerator PerformShoot()
    {
        float startAngle = -totalAngle / 2f;
        float angleStep = totalAngle / (rayCount - 1);

        Vector3[] rayDirections = new Vector3[rayCount];

        for (int i = 0; i < rayCount; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            
            Quaternion rotation = Quaternion.AngleAxis(currentAngle, Vector3.forward);
            Vector3 direction = rotation * transform.right;
            rayDirections[i] = direction;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, range);

            if (hit.collider != null)
            {
                GameObject note = hit.collider.gameObject;
                float distance = Vector3.Distance(transform.position, note.transform.position);
                if (GameData.concertGameplay.healthPoints < 350 && distance > 105f && distance < 180f)
                {
                    GameProgression.GameProgressionInstance.PlaySFX(0);
                    GameData.concertGameplay.healthPoints += 5;
                }
                else 
                {
                    GameProgression.GameProgressionInstance.PlaySFX(1);
                    GameData.concertGameplay.healthPoints -= 5;
                }
                Destroy(note);
                break;
            }
        }

        float elapsedTime = 0;
        while (elapsedTime < rayDuration)
        {
            for (int i = 0; i < rayCount; i++)
            {
                Quaternion rotation = Quaternion.AngleAxis(startAngle + (angleStep * i), Vector3.forward);
                Debug.DrawRay(transform.position, rotation * transform.right * range, Color.green);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void Rotate()
    {
        Vector2 direction = Mouse.current.position.ReadValue() - (Vector2)transform.position;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }
}