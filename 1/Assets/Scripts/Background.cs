using UnityEngine;

public class Background : MonoBehaviour
{
    private float moveSpeed = 5f;

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        if (transform.position.y < -10) {
            GameManager.instance.AddDistance(1f);
            transform.position += new Vector3(0, 20f, 0);
        }
    }
}
