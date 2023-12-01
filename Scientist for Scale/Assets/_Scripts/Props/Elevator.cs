using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Positions 
{ 
    Highest,
    AboveMiddle,
    Middle,
    BelowMiddle,
    Lowest,
}

public class Elevator : MonoBehaviour
{
    private Prop _propData;
    private Vector3 _startPos;
    [SerializeField] private float _timeToComplete;

    private Transform _transform;
    [SerializeField] private Transform _heavyPoint;
    [SerializeField] private Transform _lightPoint;
    private Vector3 _midPoint;

    private bool _isDoneMoving = true;

    [SerializeField] private bool _isConnected;
    [SerializeField] private Elevator _connectedElevator;

    public Positions _position = Positions.Middle;

    private void Awake()
    {
        _propData = GetComponent<Prop>();
        _propData.OnSizeChanged += SizeChange;
        _transform = GetComponent<Transform>();
        _midPoint = _transform.position;
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
        if (_isDoneMoving && !_isConnected)
        {
            StartCoroutine(MoveElevatorToPosition(size));
        }
        if (_isDoneMoving && _isConnected) 
        {
            SetPositionEnum(size, _connectedElevator._propData.size);
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
                    _transform.position = Vector3.Lerp(_startPos, _heavyPoint.position, timeElapsed / _timeToComplete);
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }
                _transform.position = _heavyPoint.position;
                break;
            case Size.Neutral:
                if (_transform.position != _startPos)
                {
                    while (timeElapsed < _timeToComplete)
                    {
                        _transform.position = Vector3.Lerp(_heavyPoint.position, _startPos, timeElapsed / _timeToComplete);
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

    IEnumerator MoveConnectedElevators(Vector3 endpoint)
    {
        float timeElapsed = 0;
        _isDoneMoving = false;
        _propData.RayGunLocked = true;
        Vector3 start = _transform.position;

        while (timeElapsed < _timeToComplete)
        {
            _transform.position = Vector3.Lerp(start, endpoint, timeElapsed / _timeToComplete);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        _transform.position = endpoint;
        _isDoneMoving = true;
        _propData.RayGunLocked = false;
    }

    void SetPositionEnum(Size thisSize, Size connectedSize)
    {
        switch (thisSize)
        {
            case Size.Small:
                if (connectedSize == Size.Small)
                {
                    _position = Positions.Middle;
                    StartCoroutine(MoveConnectedElevators(_midPoint));
                    _connectedElevator.StartCoroutine(_connectedElevator.MoveConnectedElevators(_connectedElevator._midPoint));
                    break;
                }
                else if (connectedSize == Size.Neutral) 
                {
                    _position = Positions.BelowMiddle;
                    StartCoroutine(MoveConnectedElevators(_midPoint + Vector3.up));
                    _connectedElevator.StartCoroutine(_connectedElevator.MoveConnectedElevators(_connectedElevator._midPoint + Vector3.down));
                    break;
                }
                else if (connectedSize == Size.Large)
                {
                    _position = Positions.Highest;
                    StartCoroutine(MoveConnectedElevators(_lightPoint.position));
                    _connectedElevator.StartCoroutine(_connectedElevator.MoveConnectedElevators(_connectedElevator._heavyPoint.position));
                    break;
                }
                break;

            case Size.Neutral:
                if (connectedSize == Size.Small)
                {
                    _position = Positions.BelowMiddle;
                    StartCoroutine(MoveConnectedElevators(_midPoint + Vector3.up));
                    _connectedElevator.StartCoroutine(_connectedElevator.MoveConnectedElevators(_connectedElevator._midPoint + Vector3.down));
                    break;
                }
                else if (connectedSize == Size.Neutral)
                {
                    _position = Positions.Middle;
                    StartCoroutine(MoveConnectedElevators(_midPoint));
                    _connectedElevator.StartCoroutine(_connectedElevator.MoveConnectedElevators(_connectedElevator._midPoint));
                    break;
                }
                else if (connectedSize == Size.Large)
                {
                    _position = Positions.AboveMiddle;
                    StartCoroutine(MoveConnectedElevators(_midPoint + Vector3.up));
                    _connectedElevator.StartCoroutine(_connectedElevator.MoveConnectedElevators(_connectedElevator._midPoint + Vector3.down));
                }
                break;

            case Size.Large:
                if (connectedSize == Size.Small)
                {
                    _position = Positions.Lowest;
                    StartCoroutine(MoveConnectedElevators(_heavyPoint.position));
                    _connectedElevator.StartCoroutine(_connectedElevator.MoveConnectedElevators(_connectedElevator._lightPoint.position));
                    break;
                }
                else if (connectedSize == Size.Neutral)
                {
                    _position = Positions.BelowMiddle;                  
                    StartCoroutine(MoveConnectedElevators(_midPoint + Vector3.down));
                    _connectedElevator.StartCoroutine(_connectedElevator.MoveConnectedElevators(_connectedElevator._midPoint + Vector3.up));
                    break;
                }
                else if (connectedSize == Size.Large)
                {
                    _position = Positions.Middle;
                    StartCoroutine(MoveConnectedElevators(_midPoint));
                    _connectedElevator.StartCoroutine(_connectedElevator.MoveConnectedElevators(_connectedElevator._midPoint));
                    break;
                }
                break;
        }
    }
}
