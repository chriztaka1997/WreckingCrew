using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float attack =20f;
    Rigidbody2D rb;
    public Transform player;

    private Vector2 moveDirection;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        moveDirection = (player.transform.position - transform.position);
        moveDirection.Normalize();
        rb.velocity = moveDirection * moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Enemy")
        {
            Destroy(gameObject);
        }
    }
}
