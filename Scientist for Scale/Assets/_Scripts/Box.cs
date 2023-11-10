using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Box : Prop, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Color _highlightColor;
    private Color _defaultColor;
    private SpriteRenderer _sr;

    private void Awake()
    {
        size = Size.Neutral;
        _sr = GetComponent<SpriteRenderer>();
        _defaultColor = Color.white;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _sr.color = _defaultColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _sr.color = _highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _sr.color = _defaultColor;
    }
}
