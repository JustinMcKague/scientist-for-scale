using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private GameObject _target;
    [SerializeField] private float _timeToAdjust;
    [SerializeField] private Vector3 _offset;

    Transform _tran;

    private void OnEnable()
    {
        GameplayManager.Instance.OnSpawn += AssignTarget;
    }

    private void OnDisable()
    {
        GameplayManager.Instance.OnSpawn -= AssignTarget;
    }

    // Start is called before the first frame update
    void Start()
    {
        _tran = transform;
    }

    void AssignTarget()
    {
        _target = GameplayManager.Instance.PlayerReference;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (_target != null)
        {
            Vector3 endPos = _target.transform.position + (_target.transform.rotation * _offset);

            Vector3 currentPos = Vector3.Slerp(transform.position, endPos, _timeToAdjust * Time.fixedDeltaTime);

            _tran.position = currentPos;
        }
    }
}