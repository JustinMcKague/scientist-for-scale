using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RaygunController : MonoBehaviour
{
    [SerializeField] PlayerInputs _inputReader = default;

    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _raygunTransform;

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

    private void Update()
    {
        RaygunPointToMouse();
    }

    private void RaygunPointToMouse()
    {
        var rayGunVector = Camera.main.WorldToScreenPoint(_raygunTransform.position);
        rayGunVector = Mouse.current.position.ReadValue() - (Vector2)rayGunVector;
        var angle = Mathf.Atan2(rayGunVector.y, rayGunVector.x) * Mathf.Rad2Deg;
        _raygunTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void FireGrowBeam()
    {
        RaycastFromMousePosition();

        if (ObjectToAffect != null && !ObjectToAffect.RayGunLocked)
        {
            ObjectToAffect.SetScale(1);
        }

        ResetGO();
    }

    private void FireShrinkBeam()
    {
        RaycastFromMousePosition();

        if (ObjectToAffect != null && !ObjectToAffect.RayGunLocked)
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
