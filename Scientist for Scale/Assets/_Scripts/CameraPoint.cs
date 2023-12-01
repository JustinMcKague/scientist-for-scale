using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPoint : MonoBehaviour
{
    [SerializeField] private Camera _sceneCamera;
    [SerializeField] private Vector3 _cameraPositionAfter;

    private Vector3 _cameraPositionBefore;

    private bool _startedMove;

    [SerializeField] private float _timeToMove;

    [SerializeField] private float _cameraScale;

    // Start is called before the first frame update
    void Start()
    {
        _cameraPositionBefore = _sceneCamera.transform.position;
    }

    IEnumerator MoveCamToLockedPosition()
    {
        float timeElapsed = 0;
        _startedMove = true;
        Vector3 startPos = _sceneCamera.transform.position;
        float startSize = _sceneCamera.orthographicSize;
        while (timeElapsed < _timeToMove)
        {
            _sceneCamera.transform.position = Vector3.Lerp(startPos, _cameraPositionAfter, timeElapsed / _timeToMove);
            _sceneCamera.orthographicSize = Mathf.Lerp(startSize, _cameraScale, timeElapsed / _timeToMove);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        _sceneCamera.transform.position = _cameraPositionAfter;
        _sceneCamera.orthographicSize = _cameraScale;
        _startedMove = false;
    }

    private void OnBecameVisible()
    {
        if (!_startedMove && CameraManager.Instance._playerInView)
        {
            CameraManager.Instance.HasCameraPointData = true;
            CameraManager.Instance.CurrentCameraPoint = this.gameObject;
            StartCoroutine(MoveCamToLockedPosition());
        }
    }
}
