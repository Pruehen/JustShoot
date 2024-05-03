using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public ObjectPoolManager poolManager;
    public Queue<GameObject> m_Queue = new Queue<GameObject>();

    public GameObject[] prefabs;
    public float spawnTime = 10.0f;
    public int m_Count = 10;

    private void Start()
    {
        //처음 생성할 때
        //poolManager.CreatePool(prefabs[0], m_Count);

        for (int i = 0; i < m_Count; i++)
        {
            //int e_rand = Random.Range(0, prefabs.Length);
            GameObject m_object = poolManager.DequeueObject(prefabs[0]);
            m_object.transform.position = transform.position;
            m_object.gameObject.SetActive(false);

            m_Queue.Enqueue(m_object);
        }
        StartCoroutine(EnemySpawn());
    }

    private IEnumerator EnemySpawn()
    {
        while (m_Queue.Count > 0)
        {
            GameObject Enemy = m_Queue.Dequeue();
            Enemy.SetActive(true);

            yield return new WaitForSeconds(spawnTime);
            //m_Queue.Enqueue(prefabs[0]);
        }

    }
}
