using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AsyncLoading : MonoBehaviour
{
    public bool active = false;
    
    private AsyncOperation operation;
    public AudioSource sc;
    [SerializeField] private AudioClip menues;

    private void Awake()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat("BG", 0.75f);
        PlayerPrefs.SetFloat("VFX", 0.5f);
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            StartCoroutine(LoadAsync(1));
        }
    }

    private void Update()
    {
        if (active == true)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                sc.volume *= PlayerPrefs.GetFloat("VFX");
                if (menues) sc.PlayOneShot(menues);
                pressActiveScene();
            }
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (PlayerPrefs.GetFloat("BG") * 100 - 0.01f < 0) return;
            PlayerPrefs.SetFloat("BG", PlayerPrefs.GetFloat("BG") - 0.1f * Time.deltaTime);
            PlayerPrefs.SetFloat("VFX", PlayerPrefs.GetFloat("VFX") - 0.1f * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            if (PlayerPrefs.GetFloat("BG") * 100 + 0.01f > 100) return;
            PlayerPrefs.SetFloat("BG", PlayerPrefs.GetFloat("BG") + 0.1f * Time.deltaTime);
            PlayerPrefs.SetFloat("VFX", PlayerPrefs.GetFloat("VFX") + 0.1f * Time.deltaTime);
        }
    }

    public IEnumerator LoadAsync(int indexScene)
    {
        yield return null;
        operation = SceneManager.LoadSceneAsync(indexScene);
        operation.allowSceneActivation = false;
        active = true;
    }
    public void pressActiveScene() { operation.allowSceneActivation = true; }
}
