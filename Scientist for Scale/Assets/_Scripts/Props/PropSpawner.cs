using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropSpawner : MonoBehaviour
{
    private Transform _transform;
    [SerializeField] private GameObject _propPrefab;

    private GameObject _spawnedGO;


    private void Awake()
    {
        _transform = transform;
    }

    private void SpawnProp()
    {
        GameObject spawnedProp = Instantiate(_propPrefab, _transform.position, Quaternion.identity);
        spawnedProp.AddComponent<DestroyBehavior>();
        _spawnedGO = spawnedProp;
    }

    // Update is called once per frame
    void Update()
    {
        if (_spawnedGO == null)
        {
            SpawnProp();
        }
    }
}
