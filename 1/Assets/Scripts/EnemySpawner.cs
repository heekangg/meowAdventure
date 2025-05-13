using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] enemies;

    [SerializeField]
    private GameObject boss;

    private bool isBossSpawned = false;

    private float[] arrPosX = {-2.1f, -0.6f, 0.6f, 2.1f };

    [SerializeField]
    private float spawnInterval = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        StartEnemyRoutine();
    }

    void StartEnemyRoutine() {
        StartCoroutine("EnemyRoutine");
    }

    public void StopEnemyRoutine() {
        StopCoroutine("EnemyRoutine");
    } 

    IEnumerator EnemyRoutine() {
        yield return new WaitForSeconds(3f);

        float moveSpeed = 1f;
        int spawnCount = 0;
        int enemyIndex = 0;

        while (true) {

            if (isBossSpawned) {
                        // 보스가 등장했으면 코루틴 종료
                        yield break;
                    }

            foreach (float posX in arrPosX) {
                SpawnEnemy(posX, enemyIndex, moveSpeed);

                // 0 ~ 2초 사이 랜덤 대기
                float randomDelay = Random.Range(0f, 2f);
                yield return new WaitForSeconds(randomDelay);

            }

            spawnCount++;
            if (spawnCount % 5 == 0) { // 5 , 10, 15, ....
                enemyIndex += 1;
                moveSpeed += 0.5f;
            }

            if (enemyIndex >= enemies.Length) {
                SpawnBoss();

                // 보스 스폰 후 약간 대기
                //yield return new WaitForSeconds(2f);

            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy(float posX, int currentMaxIndex, float moveSpeed) {
        Vector3 spawnPos = new Vector3(posX, transform.position.y, transform.position.z);

        // if (Random.Range(0, 4) == 0) { // 무기를 자리마다 랜덤으로 소환
        //     index += 1;
        // }

        // if (index >= enemies.Length) {
        //     index = enemies.Length -1;
        // }

        int randomIndex = Random.Range(0, currentMaxIndex + 1); // 0부터 currentMaxIndex까지 랜덤

        // 배열 범위 넘지 않게 방어
        randomIndex = Mathf.Clamp(randomIndex, 0, enemies.Length - 1);

        GameObject enemyObject = Instantiate(enemies[randomIndex], spawnPos, Quaternion.identity);
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        enemy.SetMoveSpeed(moveSpeed);
    }

    void SpawnBoss() {
        Instantiate(boss, transform.position, Quaternion.identity);
        isBossSpawned = true; // 보스가 등장했음을 기록
    }
}
