using UnityEngine;

public class BossWeapon : MonoBehaviour
{

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         Destroy(gameObject, 3f); // 5초 후 자동 파괴
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
