using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assignment3 : MonoBehaviour
{
    [SerializeField] private Material _explosionMaterial;
    

    private Color _startColor;
    private Color _endColor = Color.clear;
    private float _colorTransitionDuration;
    private float _moveSpeed;
    private Vector3 _moveDir;

    /// <summary>
    /// 1. Sets random speed, fade out duration, and move direction for each particle
    /// 2. Sets the initial color to Red
    /// 3. Calls out Fade Out Coroutine
    /// </summary>
    void Awake()
    {
        _moveSpeed = Random.Range(5f, 8f);
        _colorTransitionDuration = Random.Range(0.2f, 0.4f);
        _moveDir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        _explosionMaterial.color = Color.red;
        _startColor = _explosionMaterial.color;
        StartCoroutine(DestroyExplosionParticles(_startColor, _endColor));
    }

    /// <summary>
    /// Moves particle is random direction at random speed
    /// </summary>
    void Update()
    {
        transform.Translate(_moveDir * _moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Fades from current color to transparent
    /// </summary>
    /// <param name="startColor">Starting color of the Game Object</param>
    /// <param name="endColor">End color of Game Object</param>
    /// <returns></returns>
    private IEnumerator FadeOutParticle(Color startColor, Color endColor)
    {
        // Wait 2 seconds before animation starts
        yield return new WaitForSeconds(2f);

        // Start a counter for time elapsed
        float timeElapsed = 0;

        // Cache current Time.Time as startTime
        float startTime = Time.time;

        // Run fade animation for the desired amount of time
        while (timeElapsed < _colorTransitionDuration) 
        {
            // Lerp color between start and end color
            _explosionMaterial.color = Color.Lerp(startColor, endColor, timeElapsed / _colorTransitionDuration);

            // Set time elapsed
            timeElapsed = Time.time - startTime;

            // Go to next frame
            yield return null;
        }

    }

    private IEnumerator DestroyExplosionParticles(Color startColor, Color endColor)
    {
        yield return FadeOutParticle(_startColor, _endColor);
        Destroy(gameObject);
    }

}
