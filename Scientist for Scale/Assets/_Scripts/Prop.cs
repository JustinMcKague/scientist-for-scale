using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the three scale states
/// These states are: Small, Neutral, and Large
/// </summary>
public enum Size
{
    Small,
    Neutral,
    Large
}

public class Prop : MonoBehaviour
{
    private float _neutralScale = 1f;
    private float _smallScale = 0.5f;
    private float _largeScale = 1.5f;

    private Vector3 _defaultSize;

    private Size _currentSize = Size.Neutral;

    [HideInInspector] public Size size;

    private Vector3 _transformTo;

    [SerializeField] private float _scaleTime;
    private float _timeElapsed;

    private bool _scaleDone = true;

    private float _weight = 1f;
    [SerializeField] private bool _isAffectedByHeavy;
    [SerializeField] private bool _isAffectedByLight;

    private SpriteRenderer _sr;
    private Color _defaultColor;
    [SerializeField] private Color _shrinkColor;
    [SerializeField] private Color _growColor;

    private void Awake()
    {
        size = _currentSize;
        _sr = GetComponent<SpriteRenderer>();
        _defaultColor = _sr.color;
    }

    private void Start()
    {
        _defaultSize = transform.localScale;
    }

    /// <summary>
    /// Sets the scale that the prop will grow or shrink to. 
    /// 1 makes the shape grow, while -1 makes the shape shrink.
    /// </summary>
    /// <param name="scaleMultiplier"></param>
    public void SetScale(int scaleMultiplier)
    {
        switch (scaleMultiplier)
        {
            case -1:
                if (_currentSize != Size.Small)
                {
                    _transformTo = _defaultSize * (_currentSize == Size.Neutral ? _smallScale : _neutralScale);
                    _currentSize = Util.Previous(_currentSize); //Goes one position backwards in the enum list
                    ChangePropColor();
                    if (_scaleDone)
                        StartCoroutine(ScaleProp());
                }
                break;

            case 1:
                if (_currentSize != Size.Large)
                {
                    _transformTo = _defaultSize * (_currentSize == Size.Neutral ? _largeScale : _neutralScale);
                    _currentSize = Util.Next(_currentSize); //Goes one position forward in the enum list
                    ChangePropColor();
                    if (_scaleDone)
                        StartCoroutine(ScaleProp());
                }
                break;
        }
    }

    private void ChangePropColor()
    {
        switch (_currentSize) 
        {
            case Size.Neutral:
                StartCoroutine(ColorLerp(_defaultColor)); break;

            case Size.Small:
                StartCoroutine(ColorLerp(_shrinkColor)); break;

            case Size.Large:
                StartCoroutine(ColorLerp(_growColor)); break;
        }
    }

    private IEnumerator ScaleProp()
    {
        _scaleDone = false;
        Vector3 startScale = transform.localScale;
        while (_timeElapsed < _scaleTime)
        {
            transform.localScale = Vector2.Lerp(startScale, _transformTo, _timeElapsed / _scaleTime);
            _timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = _transformTo;
        _scaleDone = true;
        _timeElapsed = 0;
    }

    private IEnumerator ColorLerp(Color endColor)
    {
        Color startColor = _sr.color;
        while (_timeElapsed < _scaleTime)
        {
            _sr.color = Color.Lerp(startColor, endColor, _timeElapsed / _scaleTime);
            _timeElapsed += Time.deltaTime;
            yield return null;
        }
        _sr.color = endColor;
    }
}
