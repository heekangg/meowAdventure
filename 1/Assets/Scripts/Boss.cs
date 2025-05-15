using UnityEngine;
using System.Collections.Generic; // 리스트 사용 위해 추가
using System.Collections;

public class Boss : MonoBehaviour
{
    [SerializeField] private float maxHp = 300f;

    //보스패턴1 유도탄
    [SerializeField] private GameObject bossWeaponPrefab; // Boss 무기 프리팹 연결
    [SerializeField] private Transform shootTransform;    // Boss 무기 발사 위치
    [SerializeField] private float bossBulletSpeed = 5f;
    private float shootInterval = 1f; // 보스공격 간격
    private float lastShootTime = 0f;

    //보스패턴2 레이저
    [SerializeField] private GameObject laserPrefab; // 레이저 프리팹 연결
    private float[] laserPosX = { -2.1f, -0.6f, 0.6f, 2.1f }; // 레이저 발사 x좌표
    private List<int[]> laserWaves = new List<int[]>() {
        new int[] { 0 },     // 0번째 레이저
        new int[] { 1 },     // 1번째 레이저
        new int[] { 2 },     // 2번째 레이저
        new int[] { 0, 3 },  // 0번 + 3번 동시에
        new int[] { 1, 2 },  // 1번 + 2번 동시에
    };
    private bool isRandomLaserPhase = false; // 랜덤 모드 
    private int currentLaserIndex = 0;
    private bool isLaserInProgress = false;
    private float laserWarningTime = 1.5f; // 레이저 대기 시간 (애니메이션 맞춰서)

    [SerializeField] private int coinAmount = 30; //코인 스폰


    private float currentHp;

    private float moveSpeed = 0.5f;
    private float stopY = 3.5f; // 특정 위치
    private bool isStopped = false;
    private bool isInvincible = true; // 무적 상태 시작

    private List<System.Action> attackPatterns = new List<System.Action>();
    private int currentPatternIndex = 0;
    private bool isAttacking = false;
    private float patternDuration = 15f; // 각 패턴 유지 시간
    private float patternTimer = 0f;
    

    private void Start()
    {
        currentHp = maxHp;

        GameManager.instance.ShowBossHpBar(maxHp); // 보스 체력바 활성화
    }

    private void Update()
    {
        if (!isStopped) {
            transform.position += Vector3.down * moveSpeed * Time.deltaTime;

            if (isInvincible && transform.position.y <= stopY) {
                isInvincible = false; // 무적 해제
                StartAttackPattern(); // 공격 패턴 시작
                isStopped = true;
            }
        }

         if (isAttacking) {
            patternTimer -= Time.deltaTime;

            if (patternTimer <= 0f)
            {
                // 다음 패턴으로 넘기기
                currentPatternIndex++;
                if (currentPatternIndex >= attackPatterns.Count)
                {
                    currentPatternIndex = 0; // 순환
                }

                patternTimer = patternDuration;
            }

            attackPatterns[currentPatternIndex]?.Invoke(); // 현재 패턴 실행
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Weapon"))
        {
            if (isInvincible) return;
            Weapon weapon = other.GetComponent<Weapon>();
            TakeDamage(weapon.damage);
            Destroy(other.gameObject);
        }
    }

    private void TakeDamage(float damage)
    {
        currentHp -= damage;

        GameManager.instance.UpdateBossHpBar(currentHp); // 체력 업데이트

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.instance.HideBossHpBar(); // 보스 죽을 때 체력바 숨기기
        SpawnCoins();
        Destroy(gameObject);
    }

    private void SpawnCoins() {
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) return;

        for (int i = 0; i < coinAmount; i++)
        {
            Vector3 pos = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
            GameObject coin = Instantiate(GameManager.instance.coinPrefab, pos, Quaternion.identity);
            var item = coin.GetComponent<Item>();
            if (item != null) item.ActivateMagnet(player);
        }
    }

    private void StartAttackPattern() {
        Debug.Log("보스 공격 패턴 시작!");
        attackPatterns.Add(Pattern_ShootAtPlayer); // 첫 번째 패턴: 유도탄 등록
        attackPatterns.Add(Pattern_LaserAttack);   // 두 번째 패턴: 레이저 공격  추가!

        isAttacking = true;
        patternTimer = patternDuration;
        currentPatternIndex = 0;
        
    }

    private void Pattern_ShootAtPlayer() {
        if (Time.time - lastShootTime >= shootInterval) {
            ShootAtPlayer();
            lastShootTime = Time.time;
        }
    }

    private void ShootAtPlayer() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector3 playerPos = player.transform.position;
        Vector3 direction = (playerPos - shootTransform.position).normalized;

        GameObject bullet = Instantiate(bossWeaponPrefab, shootTransform.position, Quaternion.identity);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null) {
            rb.linearVelocity = direction * bossBulletSpeed;
        }
    }

    private void Pattern_LaserAttack() {
        if (!isLaserInProgress) {
            StartCoroutine(LaserAttackRoutine());
        }
    }

    private IEnumerator LaserAttackRoutine() {
        isLaserInProgress = true;

        // 1.5초 대기 (애니메이션 경고 시간)
        yield return new WaitForSeconds(laserWarningTime);

        if (!isRandomLaserPhase) {
            // 고정된 패턴 발사
            int[] currentWave = laserWaves[currentLaserIndex];

            foreach (int laserIndex in currentWave)
            {
                Vector3 laserPos = new Vector3(laserPosX[laserIndex], shootTransform.position.y, 0f);
                GameObject laser = Instantiate(laserPrefab, laserPos, Quaternion.identity);
                Destroy(laser, 0.8f);
            }

            // 다음 웨이브로 이동
            currentLaserIndex++;
            if (currentLaserIndex >= laserWaves.Count) {
                isRandomLaserPhase = true; // 패턴이 끝났으면 랜덤 모드 돌입
            }
        }
        else {
            // 랜덤 레이저 발사
            int randomIndex = Random.Range(0, laserPosX.Length); // 0~3 랜덤
            Vector3 laserPos = new Vector3(laserPosX[randomIndex], shootTransform.position.y, 0f);
            GameObject laser = Instantiate(laserPrefab, laserPos, Quaternion.identity);
            Destroy(laser, 0.8f);
        }

        isLaserInProgress = false;
    }

}
