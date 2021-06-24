using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemyspawn : MonoBehaviour
{
    public Enemymovement enemy;
    public GameObject EnemyHolder;
    public PlayerMB player;
    public float spawnTime = 1.0f;

    public EnemySpawnData spawnData;
    public List<Vector2Int> enemySpawnCoords;

    public DateTime lastSpawnTime;
    private Queue<Enemymovement> prefabQueue;
    public List<Enemymovement> spawnedEnemies;
    private int numEnemies;
    private int totalEnemies;
    private LevelManagerMB levelMngr;

    private System.Random rng;



    private void Awake()
    {
        prefabQueue = new Queue<Enemymovement>();
        spawnedEnemies = new List<Enemymovement>();
        numEnemies = 0;
        rng = new System.Random();
    }

    public void StartWave(PlayerMB player, EnemySpawnData spawnData, LevelManagerMB levelMngr)
    {
        this.player = player;
        this.spawnData = spawnData;
        this.levelMngr = levelMngr;
        enemySpawnCoords = new List<Vector2Int>(levelMngr.enemySpawnCoords);

        DeleteCurrentEnemies();
        InitPrefabQueue();
        lastSpawnTime = DateTime.Now;
    }

    public void InitPrefabQueue()
    {
        prefabQueue.Clear();
        List<Enemymovement> tempPrefabs = new List<Enemymovement>();
        totalEnemies = 0;
        foreach (EnemySpawnData.EnemyTypeSpawn e in spawnData.enemyTypes)
        {
            totalEnemies += e.totalEnemies;
            for (int i = 0; i < e.totalEnemies; i++)
            {
                tempPrefabs.Add(e.enemyPF);
            }
        }
        Utils.Shuffle(tempPrefabs, rng);
        foreach (Enemymovement e in tempPrefabs) prefabQueue.Enqueue(e);
    }

    public void Update()
    {
        UpdateEnemyList();
        if ((DateTime.Now - lastSpawnTime).TotalSeconds >= spawnData.timeSpawn &&
            prefabQueue.Count != 0 &&
            numEnemies < spawnData.maxEnemies)
        {
            int toSpawn = spawnData.maxEnemies - numEnemies;
            if (toSpawn > enemySpawnCoords.Count) toSpawn = enemySpawnCoords.Count;

            List<Vector2Int> tempCoords = new List<Vector2Int>(enemySpawnCoords);
            Utils.Shuffle(tempCoords, rng);
            for (int i = 0; i < toSpawn && prefabQueue.Count != 0; i++)
            {
                SpawnEnemy(prefabQueue.Dequeue(), tempCoords[i]);
            }
            lastSpawnTime = DateTime.Now;
        }
    }

    // return enemies left
    public int UpdateEnemyList()
    {
        numEnemies = 0;
        foreach (Enemymovement e in spawnedEnemies)
        {
            if (e != null) numEnemies++;
        }
        return numEnemies;
    }

    public void DeleteCurrentEnemies()
    {
        foreach (Enemymovement e in spawnedEnemies)
        {
            if (e != null) Destroy(e.gameObject);
        }
        numEnemies = 0;
    }

    public float GetWaveProgress()
    {
        if (totalEnemies == 0)
        {
            return 0.0f;
        }
        return (float)spawnedEnemies.Count / totalEnemies;
    }


    //// Start is called before the first frame update
    //void Start()
    //{
    //    StartCoroutine(enemyWave());
    //}

    //private void spawnEnemy()
    //{
    //    Enemymovement a = Instantiate(enemy, EnemyHolder.transform);
    //    a.transform.position = new Vector2(Random.Range(-19, 19), Random.Range(-19, 19));
    //    a.player = player.transform;
    //}

    private void SpawnEnemy(Enemymovement prefab, Vector2Int spawnLocation)
    {
        Enemymovement a = Instantiate(prefab, EnemyHolder.transform);
        a.transform.position = levelMngr.level.WorldLocation(spawnLocation);
        a.player = player.transform;
        spawnedEnemies.Add(a);
    }

    //IEnumerator enemyWave()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(spawnTime);
    //        spawnEnemy();
    //    }
    //}
}
