using UnityEngine;
using System.Collections;

public class UfoController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed  = 3f;
    [SerializeField] float stopY  = 3.74f;

    [Header("Laser Settings")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform laserSpawner;

    [Header("Title UI")]
    [SerializeField] private CanvasGroup titleCanvasGroup;

    private bool hasStopped = false;
    private GameObject laserInstance;
    private Animator laserAnimator;

    void Start()
    {

        // 1) 레이저 프리팹 인스턴스화 후 비활성화
        laserInstance = Instantiate(laserPrefab, laserSpawner.position, laserSpawner.rotation);
        laserAnimator = laserInstance.GetComponent<Animator>();
        laserInstance.SetActive(false);
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

            laserInstance.transform.position = laserSpawner.position;
            laserInstance.transform.rotation = laserSpawner.rotation;

            laserInstance.SetActive(true);
            laserAnimator.Play("laser", 0, 0f);

            StartCoroutine(WaitForLaserEndAndFadeInTitle());
        }
    }

    IEnumerator WaitForLaserEndAndFadeInTitle()
    {    
        // 1) “Laser” 상태 진입 대기
        while (!laserAnimator.GetCurrentAnimatorStateInfo(0).IsName("laser"))
            yield return null;

        // 2) 애니메이션 끝날 때까지 대기
        while (laserAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;

        // 3) 페이드인 코루틴
        yield return StartCoroutine(FadeCanvasGroup(titleCanvasGroup, 0f, 1f, 0.5f));

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
