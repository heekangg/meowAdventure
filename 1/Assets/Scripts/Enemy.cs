using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject explodingEffect;
    [SerializeField]
    private GameObject coin;

    [SerializeField]
    private float moveSpeed = 10f;


    private float minY = -7f;

    [SerializeField]
    private float hp = 1f;
    private float maxHp;

    [SerializeField] private Color[] damageColors = new Color[4]
    {
        Color.white,
        new Color(1f, 0.8f, 0.8f), // 살짝 붉은 단계
        new Color(1f, 0.6f, 0.6f),
        new Color(1f, 0.4f, 0.4f)
    };

    private SpriteRenderer spriteRenderer;
    public void SetMoveSpeed(float moveSpeed) {
        this.moveSpeed = moveSpeed;
    }

    void Awake()
    {
        // 초기화
        maxHp = hp;
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateColor();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        if (transform.position.y < minY) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Weapon") {

            Weapon weapon = other.gameObject.GetComponent<Weapon>();
            hp -= weapon.damage;

            UpdateColor();
            if (hp <= 0) {
                // if (gameObject.tag == "Boss") {  //보스를 enemy.cs에서 제거하고 boss.cs로 따로 관리
                //     GameManager.instance.SetGameOver();
                // }
                if (explodingEffect != null) {
                    GameObject fx = Instantiate(explodingEffect, transform.position, Quaternion.identity);
                    Destroy(fx, 1f);
                }
                Destroy(gameObject);
                Instantiate(coin, transform.position, Quaternion.identity);
            }
            Destroy(other.gameObject);
        }
    }
    private void UpdateColor()
    {
        if (spriteRenderer == null || damageColors.Length == 0) return;

        // hp 감소 비율에 따라 0~3 인덱스 선택
        float normalized = Mathf.Clamp01((maxHp - hp) / maxHp);
        int index = Mathf.FloorToInt(normalized * damageColors.Length);
        index = Mathf.Clamp(index, 0, damageColors.Length - 1);

        spriteRenderer.color = damageColors[index];
        
    }

}

