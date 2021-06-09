using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemyspawn : MonoBehaviour
{
    public Enemymovement enemy;
    public GameObject EnemyHolder;
    public GameObject player;
    public float spawnTime = 1.0f;
    private Vector2 whereToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(enemyWave());
    }

    private void spawnEnemy()
    {
        Enemymovement a = Instantiate(enemy, EnemyHolder.transform);
        a.transform.position = new Vector2(Random.Range(-19, 19), Random.Range(-19, 19));
        a.player = player.transform;
    }

    IEnumerator enemyWave()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnTime);
            spawnEnemy();  
        }
    }
}
