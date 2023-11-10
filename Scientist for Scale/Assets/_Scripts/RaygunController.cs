using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RaygunController : MonoBehaviour
{
    [SerializeField] PlayerInputs _inputReader = default;

    [SerializeField] private Transform _playerTransform;
    private Transform _raygunTransform;

    [HideInInspector] public Prop ObjectToAffect;

    private void OnEnable()
    {
        _inputReader.GrowEvent += FireGrowBeam;
        _inputReader.ShrinkEvent += FireShrinkBeam;
    }

    private void OnDisable()
    {
        _inputReader.GrowEvent -= FireGrowBeam;
        _inputReader.ShrinkEvent -= FireShrinkBeam;
    }

    private void Awake()
    {
        _raygunTransform = GetComponent<Transform>();
    }

    private void Update()
    {
        RaygunPointToMouse();
    }

    private void RaygunPointToMouse()
    {

    }

    private void FireGrowBeam()
    {
        RaycastFromMousePosition();

        if (ObjectToAffect != null)
        {
            ObjectToAffect.SetScale(1);
        }

        ResetGO();
    }

    private void FireShrinkBeam()
    {
        RaycastFromMousePosition();

        if (ObjectToAffect != null)
        {
            ObjectToAffect.SetScale(-1);
        }

        ResetGO();
    }

    private void RaycastFromMousePosition()
    {
        var mousePosInScreen = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePosInScreen, Vector3.forward, Mathf.Infinity);
        
        if (hit.collider != null)
        {
            ObjectToAffect = hit.collider.GetComponent<Prop>();
        }
    }

    private void ResetGO()
    {
        ObjectToAffect = null;
    }
}
