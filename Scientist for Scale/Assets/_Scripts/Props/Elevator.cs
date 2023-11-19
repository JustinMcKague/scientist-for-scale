using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    private Prop _propData;
    private Vector3 _startPos;
    [SerializeField] private float _timeToComplete;

    private Transform _transform;
    [SerializeField] private Transform _endPoint;

    private bool _isDoneMoving = true;

    private void Awake()
    {
        _propData = GetComponent<Prop>();
        _propData.OnSizeChanged += SizeChange;
        _transform = GetComponent<Transform>();
    }

    private void OnDisable()
    {
        _propData.OnSizeChanged -= SizeChange;
    }

    // Start is called before the first frame update
    void Start()
    {
        _startPos = transform.position;
    }

    void SizeChange(Size size)
    {
        if (_isDoneMoving)
        {
            StartCoroutine(MoveElevatorToPosition(size));
        }
    }

    IEnumerator MoveElevatorToPosition(Size size)
    {
        float timeElapsed = 0;
        _isDoneMoving = false;
        _propData.RayGunLocked = true;
        switch (size)
        {
            case Size.Large:
                while (timeElapsed < _timeToComplete)
                {
                    _transform.position = Vector3.Lerp(_startPos, _endPoint.position, timeElapsed / _timeToComplete);
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }
                _transform.position = _endPoint.position;
                break;
            case Size.Neutral:
                if (_transform.position != _startPos)
                {
                    while (timeElapsed < _timeToComplete)
                    {
                        _transform.position = Vector3.Lerp(_endPoint.position, _startPos, timeElapsed / _timeToComplete);
                        timeElapsed += Time.deltaTime;
                        yield return null;
                    }
                }
                _transform.position = _startPos;
                break;
        }
        _isDoneMoving = true;
        _propData.RayGunLocked = false;
    }
}
