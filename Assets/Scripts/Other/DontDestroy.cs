using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DontDestroy : MonoBehaviour
{
    private AudioSource sc;
    [SerializeField] private Text volume;

    [SerializeField] private AudioClip metal;
    public AudioClip menu;
    private bool pass;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        sc = GetComponent<AudioSource>();
        StartCoroutine(LerpPitch(0.75f));
    }

    private void Update()
    {
        if (!pass && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0) sc.volume = PlayerPrefs.GetFloat("BG");
        if (volume)
        {
            volume.text = $"BG: {PlayerPrefs.GetFloat("BG") * 100}\nVFX: {PlayerPrefs.GetFloat("VFX") * 100}";
        }
    }

    private IEnumerator LerpPitch(float val)
    {
        while (sc.pitch != val)
        {
            sc.pitch = Mathf.MoveTowards(sc.pitch, val, Time.deltaTime * 0.01f);
            yield return null;
        }
        StartCoroutine(LerpPitch(val == 0.75f ? 1.25f : 0.75f));
    }

    public void VictoryRules()
    {
        pass = true;
        StartCoroutine(smooth(0));

        IEnumerator smooth(float val)
        {
            if (val != 0)
            {
                sc.clip = metal;
                sc.Play();
            }
            while(sc.volume != val)
            {
                sc.volume = Mathf.MoveTowards(sc.volume, val, Time.deltaTime);
                yield return null;
            }
            if (val == 0) StartCoroutine(smooth(PlayerPrefs.GetFloat("BG")));
        }
    }

    public void DefeatRules()
    {
        StartCoroutine(smooth(0));

        IEnumerator smooth(float val)
        {
            if (val != 0)
            {
                sc.clip = menu;
                sc.Play();
            }
            while(sc.volume != val)
            {
                sc.volume = Mathf.MoveTowards(sc.volume, val, Time.deltaTime);
                yield return null;
            }
            if (val == 0) StartCoroutine(smooth(PlayerPrefs.GetFloat("BG")));
        }
    }
}