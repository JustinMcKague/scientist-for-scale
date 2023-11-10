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

    private void Awake()
    {
        size = _currentSize;
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
                    _transformTo = _defaultSize * (_currentSize == Size.Neutral ? _smallScale : _neutralScale); //if the prop is neutral set it to small else set it to neutral
                    _currentSize = Util.Previous(_currentSize); //Goes one position backwards in the enum list
                    if (_scaleDone)
                        StartCoroutine(ScaleProp());
                }
                break;

            case 1:
                if (_currentSize != Size.Large)
                {
                    _transformTo = _defaultSize * (_currentSize == Size.Neutral ? _largeScale : _neutralScale); //if the prop is neutral set it to large else set it to neutral
                    _currentSize = Util.Next(_currentSize); //Goes one position forward in the enum list
                    if (_scaleDone)
                        StartCoroutine(ScaleProp());
                }
                break;
        }
    }

    private IEnumerator ScaleProp()
    {
        _scaleDone = false;
        while (_timeElapsed < _scaleTime) //This causes an unintended "snapping" of the lerp, but I kinda like it.
        {
            transform.localScale = Vector2.Lerp(transform.localScale, _transformTo, _timeElapsed / _scaleTime * Time.deltaTime);
            _timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = _transformTo;
        _scaleDone = true;
        _timeElapsed = 0;
    }
}
