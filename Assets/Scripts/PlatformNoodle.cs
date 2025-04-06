using UnityEngine;

public class PlatformNoodle : MonoBehaviour
{
    [SerializeField] private GameObject noodlePrefab;
    private int count;
    private float[] dop;
    public bool extra;

    private int count2;

    private void Start()
    {
        if (transform.parent.localScale.x < 0) extra = true;

        dop = new float[4]
        {
            7.5f,
            9.5f,
            12,
            14
        };

        count = Random.Range(1, 4);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        if (count >= 1)
        {
            NoodleController enemy = Instantiate(noodlePrefab, new Vector2(0, 0), Quaternion.identity).GetComponent<NoodleController>();
            enemy.transform.parent = transform;

            enemy.transform.position = new Vector2(transform.position.x, transform.position.y);
            float dir;
            if (!extra)
            {
                dir = Mathf.Sign(enemy.transform.position.x - PlayerController.Instance.transform.position.x);
                enemy.transform.position = new Vector2(PlayerController.Instance.transform.position.x + dop[count2] * dir, transform.position.y + 0.39f);
            }
            else
            {
                dir = Mathf.Sign(enemy.transform.position.x - PlayerController.Instance.transform.position.x) * Mathf.Sign(-transform.position.x);
                enemy.transform.position = new Vector2(PlayerController.Instance.transform.position.x + dop[count2] * -dir, transform.position.y + 0.39f);
            }
            enemy.StartRoot(dir);

            count--;
            count2++;
            Spawn();
        }
    }
}