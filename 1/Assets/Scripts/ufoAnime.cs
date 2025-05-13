using UnityEngine;
using System.Collections;

public class UfoController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed  = 3f;
    [SerializeField] float stopY  = 3.74f;

    [Header("References")]
    [SerializeField] GameObject   laserEffect;         // 기존 레이저 이펙트
    [SerializeField] Animator      laserAnimator;       // 레이저 Animator
    [SerializeField] CanvasGroup   titleCanvasGroup;    // TitleUI에 추가한 CanvasGroup

    bool hasStopped = false;

    void Start()
    {
        laserEffect.SetActive(false);
        titleCanvasGroup.alpha           = 0f;
        titleCanvasGroup.interactable    = false;
        titleCanvasGroup.blocksRaycasts  = false;
    }

    void Update()
    {
        if (hasStopped) return;

        transform.position += Vector3.down * speed * Time.deltaTime;

        if (transform.position.y <= stopY)
        {
            hasStopped = true;
            transform.position = new Vector3(transform.position.x, stopY, transform.position.z);

            laserEffect.SetActive(true);
            laserAnimator.Play("laser", 0, 0f);

            StartCoroutine(WaitForLaserEndAndFadeInTitle());
        }
    }

    IEnumerator WaitForLaserEndAndFadeInTitle()
    {
        Debug.Log("▶ WaitForLaserEnd 시작");   // 이 로그가 찍히는지 콘솔 확인
        // 1) “Laser” 상태 진입 대기
        while (!laserAnimator.GetCurrentAnimatorStateInfo(0).IsName("laser"))
            yield return null;

        // 2) 애니메이션 끝날 때까지 대기
        while (laserAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;
        Debug.Log("▶ 애니메이션 재생 완료, 페이드 시작");
        // 3) 페이드인 코루틴
        yield return StartCoroutine(FadeCanvasGroup(titleCanvasGroup, 0f, 1f, 0.5f));
        Debug.Log("▶ 페이드 완료, 인터렉션 활성화");

        // 4) 완전히 켜진 뒤에 클릭 활성화
        titleCanvasGroup.interactable   = true;
        titleCanvasGroup.blocksRaycasts = true;
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        float elapsed = 0f;
        cg.alpha = from;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        cg.alpha = to;
    }
}
