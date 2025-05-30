using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class RandomUIAnimator : MonoBehaviour
{
    [Header("랜덤 재생 간격 (초)")]
    [SerializeField] private float minInterval = 2f;
    [SerializeField] private float maxInterval = 5f;

    [Header("눈깜빡임")]
    [SerializeField] private string triggerName = "PlayAnim";

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("Animator 컴포넌트를 찾을 수 없습니다.");
    }

    void Start()
    {
        StartCoroutine(PlayRandomly());
    }

    private IEnumerator PlayRandomly()
    {
        while (true)
        {
            // 1)랜덤 대기
            float wait = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(wait);

            // 2) 트리거로 애니메이션 재생
            animator.SetTrigger(triggerName);
        }
    }
}
