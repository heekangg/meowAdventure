using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemType itemType; // Inspector에서 설정

    // Magnet 전용
    private Transform playerTransform;
    private bool isMagnetActive = false;
    [SerializeField] private float magnetSpeed = 8f;

    void Update()
    {
        if (itemType == ItemType.Magnet && isMagnetActive && playerTransform != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                playerTransform.position,
                magnetSpeed * Time.deltaTime
            );
        }
        // Shield, Boost 등은 나중에 여기에 추가
    }


    // 외부에서 호출하여 Magnet 기능을 활성화
    public void ActivateMagnet(Transform player)
    {
        if (itemType != ItemType.Magnet) return;
        playerTransform = player;
        isMagnetActive = true;
    }
}