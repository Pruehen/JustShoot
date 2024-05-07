using System.Collections;
using System.Collections.Generic;
//using UnityEditor.EditorTools;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Queue<GameObject> m_Queue = new Queue<GameObject>();

    public GameObject[] prefabs;
    public Transform[] OuterSpawner;
    public float spawnTime = 1.0f;
    public int m_Count;

    public Queue<GameObject> m_SpecialEnemyQueue = new Queue<GameObject>();
    public GameObject[] specialEnemyPrefabs;
    public int m_SpecialEnemyCount = 15;
    public int spawnSpecialEnemyPerKill = 10;

    public float rangePosX = 2.5f;
    public float rangePosY = 2.5f;

    private void Start()
    {
        //처음 생성할 때
        //poolManager.CreatePool(prefabs[0], m_Count);

        for (int i = 0; i < m_Count; i++)
        {
            int e_rand = Random.Range(0, prefabs.Length);
            int t_rand = Random.Range(0, OuterSpawner.Length);
            GameObject m_object = ObjectPoolManager.Instance.DequeueObject(prefabs[e_rand]);

            // ;
            m_object.transform.position = positionRandom(OuterSpawner[t_rand].position);
            m_object.gameObject.SetActive(false);

            m_Queue.Enqueue(m_object);
        }
        InitSpectialEnemy();
        Player.Instance.combat.OnKill += SpawnSpecialEnemy;

        StartCoroutine(EnemySpawn());
    }

    private IEnumerator EnemySpawn()
    {
        while (m_Queue.Count > 0)
        {
            if (Player.Instance.onPlay)
            {
                GameObject Enemy = m_Queue.Dequeue();
                Enemy.SetActive(true);
            }

            yield return new WaitForSeconds(spawnTime);
            //m_Queue.Enqueue(prefabs[0]);
        }
    }
    void InitSpectialEnemy()
    {
        for (int i = 0; i < m_SpecialEnemyCount; i++)
        {
            int e_rand = Random.Range(0, specialEnemyPrefabs.Length);
            int t_rand = Random.Range(0, OuterSpawner.Length);
            GameObject m_object = ObjectPoolManager.Instance.DequeueObject(specialEnemyPrefabs[e_rand]);

            // ;
            m_object.transform.position = positionRandom(OuterSpawner[t_rand].position);
            m_object.gameObject.SetActive(false);

            m_SpecialEnemyQueue.Enqueue(m_object);
        }
    }
    void SpawnSpecialEnemy()
    {
        if (Player.Instance.onPlay && Player.Instance.combat.GetKillCount() % spawnSpecialEnemyPerKill == 0)
        {
            GameObject Enemy = m_SpecialEnemyQueue.Dequeue();
            Enemy.SetActive(true);
        }
    }

    Vector3 positionRandom(Vector3 t)
    {
        float x_rand = Random.Range(-rangePosX, rangePosX);
        float y_rand = Random.Range(-rangePosY, rangePosY);
        t.x += x_rand;
        t.y += y_rand;
        return t;
    }
}

