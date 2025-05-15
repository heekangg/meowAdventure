using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Debris : MonoBehaviour
{
    [Header("파편 이동·소멸 설정")]

    [Tooltip("이 Y값 이하로 내려가면 파편 제거")]
    [SerializeField] private float minY = -7f;

    void Update()
    {
        // // Rigidbody2D가 없을 때 수동 이동
        // if (!TryGetComponent<Rigidbody2D>(out _))
        //     transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

        // 특정 Y 이하로 내려가면 파괴
        if (transform.position.y <= minY)
            Destroy(gameObject);
    }


}
