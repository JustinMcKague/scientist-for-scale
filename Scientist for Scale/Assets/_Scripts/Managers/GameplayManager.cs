using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameplayManager : StaticInstance<GameplayManager>
{
    public GameObject PlayerPrefab;

    public Transform PlayerSpawnTransform;
    private GameObject _spawnedPlayer;

    public event UnityAction OnSpawn;

    [HideInInspector] public GameObject PlayerReference;

    private void Start()
    {
        _spawnedPlayer = Instantiate(PlayerPrefab, PlayerSpawnTransform.position, Quaternion.identity);
        PlayerReference = _spawnedPlayer;
        OnSpawn?.Invoke();
    }
}
