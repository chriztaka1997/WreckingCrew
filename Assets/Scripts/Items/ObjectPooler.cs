using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ObjectPoolItem
{
    public int amountToPool;
    public GameObject objectToPool;
    public bool shouldExpand;
    public string itemName;
    public float zPos;
}


public class ObjectPooler : MonoBehaviour
{

    public static ObjectPooler SharedInstance;
    private int currentCoin;
    public Text coinText;
    public List<ObjectPoolItem> itemsToPool;
    public List<GameObject> pooledObjects;

    private void Awake()
    {
        SharedInstance = this;
        currentCoin = 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        pooledObjects = new List<GameObject>();
        Transform itemHolder = GameObject.Find("ItemHolder").transform;
        foreach (ObjectPoolItem item in itemsToPool)
        {
            GameObject subHolder = new GameObject(item.itemName);
            subHolder.transform.SetParent(itemHolder);
            for (int i = 0; i < item.amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(item.objectToPool, subHolder.transform);
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
        UpdateCoin(0);
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetPooledObject(string tag)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].CompareTag(tag))
            {
                return pooledObjects[i];
            }
        }
        foreach (ObjectPoolItem item in itemsToPool)
        {
            if (item.objectToPool.CompareTag(tag))
            {
                if (item.shouldExpand)
                {
                    GameObject obj = (GameObject)Instantiate(item.objectToPool);
                    obj.SetActive(false);
                    pooledObjects.Add(obj);
                    return obj;
                }
            }
        }
        return null;
    }


    public void SpawnCoin(Vector3 pos)
    { 
        GameObject coin = SharedInstance.GetPooledObject("Coin");
        if (coin != null)
        {
            coin.SetActive(true);
            pos.z = itemsToPool.Find(item => item.itemName == "Coin").zPos;
            coin.transform.position = pos;
        }
    }
    
    public void SpawnHealthPotion(Vector3 pos)
    {
        GameObject HP = SharedInstance.GetPooledObject("HealthPotion");
        if (HP != null)
        {
            HP.SetActive(true);
            pos.z = itemsToPool.Find(item => item.itemName == "HealthPotion").zPos;
            HP.transform.position = pos;
        }
    }

    public void SpawnSpeedElixir(Vector3 pos)
    {
        GameObject HP = SharedInstance.GetPooledObject("SpeedElixir");
        if (HP != null)
        {
            HP.SetActive(true);
            pos.z = itemsToPool.Find(item => item.itemName == "SpeedElixir").zPos;
            HP.transform.position = pos;
        }
    }

    public void SpawnStrengthElixir(Vector3 pos)
    {
        GameObject HP = SharedInstance.GetPooledObject("StrengthElixir");
        if (HP != null)
        {
            HP.SetActive(true);
            pos.z = itemsToPool.Find(item => item.itemName == "StrengthElixir").zPos;
            HP.transform.position = pos;
        }
    }

    public void DestroyCoin(GameObject gameObject) 
    {
        gameObject.SetActive(false);
    }

    public Vector2 DropLocation(Vector2 enemyLocation)
    {
        (int, int)? tile;
        Vector2 dl;
        do
        {
            float delta_x = Random.Range(0f, 1f);
            float delta_y = Random.Range(0f, 1f);
            dl = new Vector2(enemyLocation.x + delta_x, enemyLocation.y + delta_y);
            tile = GameManagerMB.instance.levelMngr.level.GridLocation(dl);
        } while (tile == null || GameManagerMB.instance.levelMngr.level.tiles[tile.Value.Item1,tile.Value.Item2].tileType != TileType.empty);
        return dl;
    }

    public void DropByEnemy(Vector3 enemyLocation, float dropingRate)
    {
        float r = Random.Range(0f, 1f);
        if(r <= dropingRate)
            SharedInstance.SpawnCoin(DropLocation(enemyLocation));
        r = Random.Range(0f, 1f);
        if (r <= dropingRate)
            SharedInstance.SpawnHealthPotion(DropLocation(enemyLocation));
        r = Random.Range(0f, 1f);
        if (r <= dropingRate)
            SharedInstance.SpawnSpeedElixir(DropLocation(enemyLocation));
/*        r = Random.Range(0f, 1f);
        if (r <= dropingRate)
            SharedInstance.SpawnStrengthElixir(enemyLocation);*/

    }

    public void UpdateCoin(int coin)
    {
        currentCoin += coin;
        coinText.text = currentCoin.ToString();
    }

    public void DestroyAllPooledObjects()
    {
        foreach (GameObject g in pooledObjects)
        {
            g.SetActive(false);
        }
    }
}
