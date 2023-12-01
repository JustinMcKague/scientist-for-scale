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
    [SerializeField] private float _neutralScale = 1f;
    [SerializeField] private float _smallScale = 0.5f;
    [SerializeField] private float _largeScale = 1.5f;

    private Vector3 _defaultSize;

    private Size _currentSize = Size.Neutral;

    [HideInInspector] public Size size;

    private Vector3 _transformTo;

    [SerializeField] private float _scaleTime;
    private float _timeElapsed;

    private bool _scaleDone = true;

    [SerializeField] private bool _canBePushed;

    [HideInInspector] public bool CanBePushed;

    [SerializeField] private bool _isAffectedByWeight;
    public bool IsLightEnough { get; private set; }

    public bool IsHeavyEnough { get; private set; }

    public bool RayGunLocked;

    public event Action<Size> OnSizeChanged;

    [SerializeField] private LayerMask _environmentLayer;

    private SpriteRenderer _sr;
    private Color _defaultColor;
    [SerializeField] private Color _shrinkColor;
    [SerializeField] private Color _growColor;
    private Rigidbody2D _rb;

    private void Awake()
    {
        CanBePushed = _canBePushed;
        size = _currentSize;
        _sr = GetComponent<SpriteRenderer>();
        _defaultColor = _sr.color;
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _defaultSize = transform.localScale;
        if (_isAffectedByWeight)
        {
            IsLightEnough = false;
        }
    }

    private void Update()
    {
        //Debug.DrawRay(transform.position, Vector3.right * (transform.localScale.x / 2 + 0.15f), Color.red);
        //CheckForOverlap();
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

                    if (_currentSize == Size.Small && _isAffectedByWeight) IsLightEnough = true;
                    if (_currentSize == Size.Neutral && _isAffectedByWeight) IsLightEnough = false; IsHeavyEnough = false;
                    size = _currentSize;
                    OnSizeChanged?.Invoke(_currentSize);

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

                    if (_isAffectedByWeight) IsLightEnough = false;
                    if (_isAffectedByWeight && _currentSize == Size.Large) IsHeavyEnough = true;
                    size = _currentSize;
                    OnSizeChanged?.Invoke(_currentSize);

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
            CheckForOverlap();
            yield return null;
        }
        transform.localScale = _transformTo;
        _scaleDone = true;
        _timeElapsed = 0;
    }

    void CheckForOverlap()
    {
        if (Physics2D.Raycast(transform.position, Vector3.right, transform.localScale.x / 2 + 0.1f, _environmentLayer))
        {
            transform.position -= Vector3.right * 0.1f;
        }
        else if (Physics2D.Raycast(transform.position, Vector3.left, transform.localScale.x / 2 + 0.1f, _environmentLayer))
        {
            transform.position += Vector3.right * 0.1f;
        }
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
