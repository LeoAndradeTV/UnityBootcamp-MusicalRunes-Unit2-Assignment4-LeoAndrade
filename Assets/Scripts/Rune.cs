using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rune : MonoBehaviour
{
    private static readonly Color hintColor = new Color(1, 1, 1, 0.6f);
    [SerializeField] private Color _activationColor;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Image _image;
    [SerializeField] private float _colorTransitionDuration = 0.3f;
    [SerializeField] private float _minActivationDuration = 0.5f;
    [SerializeField] private Button _button;

    public int Index
    {
        get;
        private set;
    }
    private Coroutine animationCoroutine;


    public void OnClick()
    {
        GameManager.Instance.OnRuneActivated(Index);
        StartCoroutine(ActivateRuneCoroutine());
    }

    public void Setup(int runeIndex)
    {
        Index = runeIndex;
        transform.SetSiblingIndex(Index);
    }

    public void DisableInteraction()
    {
        _button.interactable = false;
    }
    public void EnableInteraction()
    {
        _button.interactable = true;
    }

    public Coroutine ActivateRune()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        animationCoroutine = StartCoroutine(ActivateRuneCoroutine());
        return animationCoroutine;
    }

    public Coroutine SetHintVisual(bool state)
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        if (state)
        {
            StartCoroutine(LerpToColor(Color.white, hintColor));
        } else
        {
            animationCoroutine = StartCoroutine(LerpToColor(hintColor, Color.white));
        }
        return animationCoroutine;
    }

    public IEnumerator ActivateRuneCoroutine()
    {
        _audioSource.Play();

        yield return LerpToColor(Color.white, _activationColor);

        yield return new WaitForSeconds(_minActivationDuration);

        var duration = _audioSource.clip.length;
        while (_audioSource.isPlaying)
        {
            yield return new WaitForSeconds(duration - _audioSource.time);
        }

        yield return LerpToColor(_activationColor, Color.white);

    }

    private IEnumerator LerpToColor(Color startColor, Color endColor)
    {
        float elapsedTime = 0;
        float startTime = Time.time;

        while (elapsedTime < _colorTransitionDuration)
        {
            _image.color = Color.Lerp(startColor, endColor, elapsedTime / _colorTransitionDuration);
            elapsedTime = Time.time - startTime;
            yield return null;
        }
    }
}
