using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private GameObject[] weapons;
    private int weaponIndex = 0;

    [SerializeField]
    private Transform shootTransform;

    [SerializeField]
    private float shootInterval = 0.05f;
    private float lastShotTime = 0f;
    // Update is called once per frame
    void Update()
    {

        if (Time.timeScale == 0f) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 위치값을 카메라 기준으로 바꿔주는 코드
        float toX = Mathf.Clamp(mousePos.x, -2.35f, 2.35f); // 마우스가 일정 좌표로 나가면 플레이어가 마우스를 안따라감
        transform.position = new Vector3(toX, transform.position.y, transform.position.z);

        if (GameManager.instance.isGameOVer == false) {
            Shoot();
        }
    }

    void Shoot() {
        // 10 - 0 > 0.05
        // lastShotTime = 10;

        //10.01 - 10 > 0.05 
        //false
        if (Time.time - lastShotTime > shootInterval) {
            Instantiate(weapons[weaponIndex], shootTransform.position, Quaternion.identity);
            lastShotTime = Time.time;//현재시간으로 업데이트
        }

        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "BossWeapon" || other.gameObject.tag == "Debris") {
            GameManager.instance.SetGameOver();
            Destroy(gameObject);
        } else if (other.gameObject.tag == "Coin") {
            GameManager.instance.IncreaseCoin();
            Destroy(other.gameObject);
        }
        
    }

    public void Upgrade() {
        weaponIndex += 1;
        if (weaponIndex >= weapons.Length) {
            weaponIndex = weapons.Length - 1;
        }
    }
}
