using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameplayManager : StaticInstance<GameplayManager>
{
    public GameObject PlayerPrefab;

    public Transform PlayerSpawnTransform;
    private GameObject _spawnedPlayer;

    public event UnityAction OnSpawn;

    [HideInInspector] public GameObject PlayerReference;

    public Vector3 LastCheckpoint;
    public Sprite ActiveCheckpoint;
    public Sprite InactiveCheckpoint;

    public List<Checkpoint> Checkpoints = new List<Checkpoint>();

    private void Start()
    {
        _spawnedPlayer = Instantiate(PlayerPrefab, PlayerSpawnTransform.position, Quaternion.identity);
        PlayerReference = _spawnedPlayer;
        LastCheckpoint = PlayerSpawnTransform.position;
        OnSpawn?.Invoke();
    }

    public void UpdateAllCheckpoints()
    {
        for (int i = 0; i < Checkpoints.Count; i++)
        {
            if (Checkpoints[i].transform.position != LastCheckpoint) 
            {
                Checkpoints[i].ChangeSprite(InactiveCheckpoint);
            }
            else Checkpoints[i].ChangeSprite(ActiveCheckpoint);
        }
    }

    public void GoToScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }
}
