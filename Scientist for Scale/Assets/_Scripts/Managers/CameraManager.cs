using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : PersistentSingleton<CameraManager>
{
    [HideInInspector] public bool HasCameraPointData;
    [HideInInspector] public GameObject CurrentCameraPoint;
    public Camera Camera;
    private float _defaultCamSize;

    [SerializeField] private float _timeToReturn;

    [HideInInspector] public bool _coroutineRunning;
    [HideInInspector] public bool _playerInView = true;

    private void Start()
    {
        Camera = Camera.main;
        _defaultCamSize = Camera.orthographicSize;
    }

    public void UpdateCameraSize()
    {
        if (!_coroutineRunning)
        {
            StartCoroutine(CameraSizeChange());
        }
    }

    IEnumerator CameraSizeChange()
    {
        float timeElapsed = 0;
        _coroutineRunning = true;
        float startSize = Camera.orthographicSize;
        while (timeElapsed < _timeToReturn)
        {
            Camera.orthographicSize = Mathf.Lerp(startSize, _defaultCamSize, timeElapsed / _timeToReturn);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        Camera.orthographicSize = _defaultCamSize;
        _coroutineRunning = false;
    }
}
