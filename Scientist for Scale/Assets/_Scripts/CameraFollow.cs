using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private GameObject _target;
    [SerializeField] private float _timeToAdjust;
    [SerializeField] private Vector3 _offset;

    Transform _tran;

    void Awake()
    {
        GameplayManager.Instance.OnSpawn += AssignTarget;
        _tran = transform;
    }

    void AssignTarget()
    {
        _target = GameplayManager.Instance.PlayerReference;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (_target != null && !CameraManager.Instance.HasCameraPointData)
        {
            Vector3 endPos = _target.transform.position + (_target.transform.rotation * _offset);

            Vector3 currentPos = Vector3.Slerp(transform.position, endPos, _timeToAdjust * Time.fixedDeltaTime);

            _tran.position = currentPos;
        }
    }
}