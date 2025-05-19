using UnityEngine;
using System.Collections;

public class DebrisSpawner : MonoBehaviour
{
    [Header("파편 설정")]
    [SerializeField] private GameObject debrisPrefab;  // 파편 프리팹
    [SerializeField] private float moveSpeed    = 10f; // 낙하 속도

    [Header("스폰 간격 (초)")]
    [SerializeField] private float minInterval  = 1f;
    [SerializeField] private float maxInterval  = 4f;

    [Header("스폰 시작 딜레이 (초)")]
    [SerializeField] private float initialDelay = 20f;

    // EnemySpawner와 동일한 X 좌표 네 방향
    private readonly float[] arrPosX = { -2.1f, -0.7f, 0.7f, 2.1f };



    void Start()
    {
        StartCoroutine(SpawnDebrisRoutine());
    }

    public void StopDebrisRoutine()
    {
        StopCoroutine(SpawnDebrisRoutine());
    }

    private IEnumerator SpawnDebrisRoutine()
    {

        // 1) 게임 시작 후 initialDelay만큼 기다린 뒤 스폰 시작
        yield return new WaitForSeconds(initialDelay);
        while (true)
        {
            // 2) 보스가 존재하는 동안 스폰 중단
            if (GameObject.FindGameObjectsWithTag("Boss").Length > 0)
            {
                // 보스가 완전히 사라질 때까지 대기
                yield return new WaitUntil(() => 
                    GameObject.FindGameObjectsWithTag("Boss").Length == 0
                );
                // 보스 격파 후 다시 initialDelay 만큼 대기
                yield return new WaitForSeconds(initialDelay);
            }

            // 3) 다음 파편 스폰 대기 (1~4초 랜덤)
            float wait = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(wait);

            // 4) 파편 스폰
            float x = arrPosX[Random.Range(0, arrPosX.Length)];
            Vector3 spawnPos = new Vector3(x, transform.position.y, 0f);
            GameObject d = Instantiate(debrisPrefab, spawnPos, Quaternion.identity);

            // 속도 설정 (Rigidbody2D 가 있을 때)
            if (d.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.linearVelocity = Vector2.down * moveSpeed;
            }
        }
    }
}
