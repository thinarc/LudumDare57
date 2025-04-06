using UnityEngine;
using System.Collections;

public class TriggerVictory : MonoBehaviour
{
    [SerializeField] private GameObject victoryPanel;
    public bool twoScenary;

    public AudioSource sc;
    public AudioClip back;
    public AudioClip front;

    private void Start()
    {
        sc.volume *= PlayerPrefs.GetFloat("VFX");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (twoScenary)
            {
                FindAnyObjectByType<DontDestroy>()?.VictoryRules();
                return;
            }
            collision.gameObject.GetComponent<PlayerController>().disableMove = true;
            victoryPanel.SetActive(true);
            StartCoroutine(Wait());

            IEnumerator Wait()
            {
                yield return new WaitForSeconds(3.75f);
                sc.PlayOneShot(back);
                sc.PlayOneShot(front);
            }
        }
    }
}