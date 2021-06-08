using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemyspawn : MonoBehaviour
{
    public GameObject enemy;
    public float spawnTime = 1.0f;
    private Vector2 whereToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(enemyWave());
    }

    private void spawnEnemy()
    {
        GameObject a = Instantiate(enemy) as GameObject;
        a.transform.position = new Vector2(Random.Range(-19, 19), Random.Range(-19, 19));
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
