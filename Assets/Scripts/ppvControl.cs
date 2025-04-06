using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.SceneManagement;

using UnityEngine.Rendering.Universal;

public class ppvControl : MonoBehaviour
{
    private CinemachineVolumeSettings cvs;

    private Vignette mVignette;
    private ChannelMixer mixer;
    private LiftGammaGain gamma;

    [Header("Debug")]
    [SerializeField] private float oldVignette;
    [SerializeField] private float oldMixer;

    private void Start()
    {
        NoodleController.randSpeed = new Vector2(0.375f, 0.9f);
        NoodleController.randScale = new Vector2(0.7f, 1.2f);

        cvs = GetComponent<CinemachineVolumeSettings>();   

        for (int i = 0; i < cvs.Profile.components.Count; i++)
        {
            if (cvs.Profile.components[i].name == "Vignette")
            {
                mVignette = (Vignette)cvs.Profile.components[i];
                oldVignette = mVignette.intensity.value;
            }
            if (cvs.Profile.components[i].name == "ChannelMixer")
            {
                mixer = (ChannelMixer)cvs.Profile.components[i];
                oldMixer = mixer.redOutRedIn.value;
            }
            if (cvs.Profile.components[i].name == "LiftGammaGain")
            {
                gamma = (LiftGammaGain)cvs.Profile.components[i];
            }
        }

        StartCoroutine(ShowPPV());
    }

    public void DefeatPP()
    {
        gamma.lift.value = new Vector4(1f, 0, 0, 0);
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        operation.allowSceneActivation = false;
        StartCoroutine(Wait());

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(2.25f);
            operation.allowSceneActivation = true;
        }
    }

    private void OnDisable()
    {
        if (!mVignette || !mixer) return;
        mVignette.intensity.value = oldVignette;
        mixer.redOutRedIn.value = oldMixer;
        gamma.lift.value = new Vector4(0, 0, 0, 0);
    }


    private IEnumerator ShowPPV()
    {
        while (mVignette.intensity.value != 0.3f)
        {
            mVignette.intensity.value = Mathf.MoveTowards(mVignette.intensity.value, 0.28f, Time.deltaTime * 0.1f);
            yield return null;
        }
    }

    private void Update()
    {
        if (PlayerController.Instance)
        {
            if (PlayerController.Instance.transform.position.y < -30f && PlayerController.Instance.transform.position.y > -100f)
            {
                mixer.redOutRedIn.value = Mathf.MoveTowards(mixer.redOutRedIn.value, 26, Time.deltaTime * 1.55f);
                NoodleController.randSpeed = new Vector2(0.375f, 0.9f);
                NoodleController.randScale = new Vector2(0.7f, 1.2f);
            }
            if (PlayerController.Instance.transform.position.y < -100f && PlayerController.Instance.transform.position.y > -200f)
            {
                mixer.redOutRedIn.value = Mathf.MoveTowards(mixer.redOutRedIn.value, 35, Time.deltaTime * 3f);
                NoodleController.randSpeed = new Vector2(0.375f, 0.9f);
                NoodleController.randScale = new Vector2(0.7f, 1.2f);
            }
            if (PlayerController.Instance.transform.position.y < -200f && PlayerController.Instance.transform.position.y > -259f)
            {
                mixer.redOutRedIn.value = Mathf.MoveTowards(mixer.redOutRedIn.value, 50, Time.deltaTime * 5f);
                NoodleController.randSpeed = new Vector2(0.375f, 0.9f);
                NoodleController.randScale = new Vector2(0.7f, 1.2f);
            }
            if (PlayerController.Instance.transform.position.y < -259f && PlayerController.Instance.transform.position.y > -300f)
            {
                mixer.redOutRedIn.value = Mathf.MoveTowards(mixer.redOutRedIn.value, 76, Time.deltaTime * 7f);
                NoodleController.randSpeed = new Vector2(0.4f, 1.1f);
                NoodleController.randScale = new Vector2(0.8f, 1.2f);
            }
            if (PlayerController.Instance.transform.position.y < -300f && PlayerController.Instance.transform.position.y > -323f)
            {
                mixer.redOutRedIn.value = Mathf.MoveTowards(mixer.redOutRedIn.value, 89, Time.deltaTime * 11f);
                NoodleController.randSpeed = new Vector2(0.5f, 1.23f);
                NoodleController.randScale = new Vector2(0.9f, 1.25f);
            }
            if (PlayerController.Instance.transform.position.y < -323f && PlayerController.Instance.transform.position.y > -359f)
            {
                mixer.redOutRedIn.value = Mathf.MoveTowards(mixer.redOutRedIn.value, 94, Time.deltaTime * 20f);
                NoodleController.randSpeed = new Vector2(0.5f, 1.3f);
                NoodleController.randScale = new Vector2(1f, 1.3f);
            }
            if (PlayerController.Instance.transform.position.y < -359f && PlayerController.Instance.transform.position.y > -400f)
            {
                mixer.redOutRedIn.value = Mathf.MoveTowards(mixer.redOutRedIn.value, 136, Time.deltaTime * 30);
                NoodleController.randSpeed = new Vector2(0.5f, 1.35f);
                NoodleController.randScale = new Vector2(1f, 1.35f);
            }
            if (PlayerController.Instance.transform.position.y < -380f)
            {
                mixer.redOutRedIn.value = Mathf.MoveTowards(mixer.redOutRedIn.value, 165, Time.deltaTime * 50f);
                NoodleController.randSpeed = new Vector2(0.5f, 1.4f);
                NoodleController.randScale = new Vector2(1f, 1.4f);
            }
        }
    }
}
