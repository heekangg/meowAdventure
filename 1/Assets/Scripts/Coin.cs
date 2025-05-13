using UnityEngine;

public class Coin : MonoBehaviour
{

    private float minY = -7f;
    private float boundaryX = 2.5f;
    private Rigidbody2D rigidBody;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();//GetComponent<T>()는 Unity 내부에서 오버헤드가 큰 연산.
        // 이걸 필요할 때마다 매번 호출하면 퍼포먼스가 떨어질 수 있다.

        Jump();
    }


    void Jump() {
        // Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();

        float randomJumpForce = UnityEngine.Random.Range(4f, 8f);
        Vector2 jumpvelocity = Vector2.up * randomJumpForce;
        jumpvelocity.x = UnityEngine.Random.Range(-2f, 2f);

        rigidBody.AddForce(jumpvelocity, ForceMode2D.Impulse);
    }
    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < minY) {
            Destroy(gameObject);
        }

        // 화면 경계를 넘었을 때 반사
        if (Mathf.Abs(transform.position.x) > boundaryX)
        {
            Vector2 velocity = rigidBody.linearVelocity;
            velocity.x = -velocity.x; // x 방향 반전
            rigidBody.linearVelocity = velocity;

            // 경계 안으로 강제 이동 (튕기자마자 살짝 안쪽으로 밀어넣음)
            float clampedX = Mathf.Clamp(transform.position.x, -boundaryX, boundaryX);
            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
        }
    }
}
