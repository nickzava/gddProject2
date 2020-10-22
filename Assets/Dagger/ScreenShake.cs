using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public void ShakeScreen(float time, float speed, float shakeRadious)
    {
        StartCoroutine(Shake(time, speed, shakeRadious));
    }
    
    private IEnumerator Shake(float time, float speed, float shakeRadious)
    {
        Vector3 startPos = transform.position;
        Vector3 direction = new Vector2(Random.value-.5f, Random.value-.5f).normalized;
        while(time > 0)
        {
            transform.position += direction * speed * Time.deltaTime;
            if((startPos-transform.position).magnitude > shakeRadious)
            {
                direction = startPos-transform.position;
                direction.y *= Random.value * 4 - 2f;
                direction.x *= Random.value * 4 - 2f;
                direction = direction.normalized;
            }
            time -= Time.deltaTime;
            yield return null;
        }

        Vector3 toPos = (startPos - transform.position).normalized;
        float distToMove = speed * Time.deltaTime;
        while ((startPos - transform.position).magnitude < distToMove)
        {
            transform.position += toPos * distToMove;
            distToMove = speed * Time.deltaTime;
        }
        transform.position = startPos;
        yield break;
    }
}
