using UnityEngine;
using System;

public class NoodleController : MonoBehaviour, INoodleController
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float distanceView;

    [SerializeField, Space(5)] private float walkSpeed;
    [SerializeField] private float runSpeed;
    
    private Rigidbody2D rb;
    
    [SerializeField] private float direction;

    [SerializeField] private bool viewTarget;

    public Vector2 FrameDirection => new Vector2(rb.linearVelocity.x, rb.linearVelocity.y);
    public event Action LeaveGround;
    public event Action Catch;
    public bool ViewTarget => viewTarget;

    private bool faceRight;

    [Header("Random")]
    public static Vector2 randSpeed;
    public static Vector2 randScale;

    public void StartRoot(float scaleDir)
    {
        rb = GetComponent<Rigidbody2D>();

        transform.localScale = new Vector2(transform.localScale.x * scaleDir, transform.localScale.y);

        if (transform.localScale.x > 0)
        {
            faceRight = false;
        }
        else
        {
            faceRight = true;
        }

        direction = Mathf.Sign(-transform.localScale.x);

        if (rb.linearVelocity.x > 0 && !faceRight)
        {
            Flip();
        }
        else if (rb.linearVelocity.x < 0 && faceRight)
        {
            Flip();
        }

        float speed = UnityEngine.Random.Range(randSpeed.x, randSpeed.y);
        float scale = UnityEngine.Random.Range(randScale.x, randScale.y);

        transform.localScale = new Vector2(transform.localScale.x * scale, transform.localScale.y * scale);
        walkSpeed *= speed;
        runSpeed *= speed;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2((!viewTarget ? runSpeed : walkSpeed) * direction * 30 * Time.deltaTime, rb.linearVelocity.y);

        float face = transform.localScale.x < 0 ? 1 : -1;
        Vector2 start = transform.position;
        start.y += 1;
        if (catched)
        {
            viewTarget = false;
            return;
        }
        viewTarget = Physics2D.Raycast(start, new Vector2(face, 0.01f), distanceView, playerLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;

        float face = transform.localScale.x < 0 ? 1 : -1;
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (distanceView * face), transform.position.y + 0.37f));
    }

    private bool startHit;
    private bool invokeLeave;
    private void Update()
    {
        Vector2 scale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        // bool groundHit = Physics2D.CapsuleCast(col.bounds.center, col.size * scale, col.direction, 0, Vector2.down, 0.2f, groundLayer);

        bool groundHit = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);

        if (groundHit) startHit = true;
        if (!groundHit && startHit && !invokeLeave)
        {
            LeaveGround?.Invoke();
            invokeLeave = true;
        }

        if (catched)
        {
            PlayerController.Instance.disableMove = true;
            direction = 0;
        }
    }

    private void Flip()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        faceRight = !faceRight;
    }

    private bool catched;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player" && !catched && !invokeLeave && FindAnyObjectByType<PlayerController>().catchedPl == false)
        {
            Catch?.Invoke();
            collision.gameObject.GetComponent<PlayerController>().disableMove = true;
            catched = true;
            collision.gameObject.GetComponent<PlayerController>().animLink.SetBool("Away", true);
            collision.gameObject.GetComponent<PlayerController>().animLink.SetTrigger("TrigAway");
            collision.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            direction = 0;
            FindAnyObjectByType<ppvControl>().DefeatPP();
            FindAnyObjectByType<PlayerController>().catchedPl = true;
            FindAnyObjectByType<DontDestroy>()?.DefeatRules();
        }
    }
}

public interface INoodleController
{
    public Vector2 FrameDirection { get; }
    public event Action LeaveGround;
    public event Action Catch;
    public bool ViewTarget { get; }
}