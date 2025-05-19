using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject boss;

    private bool isBossSpawned = false;
    private readonly float[] arrPosX = { -2.1f, -0.7f, 0.7f, 2.1f };
    [SerializeField] private float spawnInterval = 1f;

    // 첫 보스까지 필요한 웨이브 수
    private int wave = 40;
    // 보스 격파할 때마다 웨이브 수에 더할 값
    private int waveIncrease = 5;
    // 지금까지 격파한 보스 수 (다음 사이클 enemyIndex 초기값으로 사용)
    private int bossClearCount = 0;

    void Start()
    {
        StartCoroutine(EnemyRoutine());
    }

    public void StopEnemyRoutine()
    {
        StopCoroutine(EnemyRoutine());
    }

    private IEnumerator EnemyRoutine()
    {
        yield return new WaitForSeconds(3f);

        float moveSpeed = 1f;
        int spawnCount = 0;
        int enemyIndex = 0; // 스폰 가능한 최대 적 인덱스

        while (true)
        {
            // 한 웨이브(4방향) 스폰
            foreach (float posX in arrPosX)
            {
                SpawnEnemy(posX, enemyIndex, moveSpeed);
                yield return new WaitForSeconds(Random.Range(0f, 2f));
            }

            spawnCount++;

            // 10의 배수 웨이브마다 적 레벨(인덱스) +1, 속도 증가
            if (spawnCount % 10 == 0)
            {
                enemyIndex++;
                moveSpeed += 0.5f;
            }

            // 보스 소환 조건: 웨이브 수가 기준 이상이고, 아직 보스가 없을 때
            if (!isBossSpawned && spawnCount >= wave)
            {
                SpawnBoss();

                // 보스가 씬에서 완전히 사라질 때까지 대기
                yield return new WaitUntil(() =>
                    GameObject.FindGameObjectsWithTag("Boss").Length == 0
                );

                isBossSpawned = false;

                // 보스 격파 후 처리
                bossClearCount++;            // 다음 사이클 시작 enemyIndex = bossClearCount
                enemyIndex = bossClearCount; // 초기화
                wave += waveIncrease;        // 다음 보스까지 필요한 웨이브 수 +5
                spawnCount = 0;              // 웨이브 카운트 리셋

                yield return new WaitForSeconds(2f);
                continue;
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy(float posX, int currentMaxIndex, float moveSpeed)
    {
        Vector3 spawnPos = new Vector3(posX, transform.position.y, transform.position.z);
        int randomIndex = Random.Range(0, currentMaxIndex + 1);
        randomIndex = Mathf.Clamp(randomIndex, 0, enemies.Length - 1);

        GameObject enemyObject = Instantiate(enemies[randomIndex], spawnPos, Quaternion.identity);
        if (enemyObject.TryGetComponent<Enemy>(out var enemy))
            enemy.SetMoveSpeed(moveSpeed);
    }

    private void SpawnBoss()
    {
        Instantiate(boss, transform.position, Quaternion.identity);
        isBossSpawned = true;
    }
}
