using UnityEngine;
using Unity.Cinemachine;

public class wobbleMulti : MonoBehaviour
{
    private CinemachineBasicMultiChannelPerlin noise;
    private float startNoise;

    private void Start()
    {
        noise = GetComponent<CinemachineBasicMultiChannelPerlin>();
        startNoise = noise.FrequencyGain;
    }

    private void Update()
    {
        if (PlayerController.Instance)
        {
            if (PlayerController.Instance.transform.position.y < -30f)
            {
                noise.FrequencyGain = startNoise * 2;
            }
            if (PlayerController.Instance.transform.position.y < -100f)
            {
                noise.FrequencyGain = startNoise * 5;
            }
            if (PlayerController.Instance.transform.position.y < -200f)
            {
                noise.FrequencyGain = startNoise * 7;
            }
            if (PlayerController.Instance.transform.position.y < -259f)
            {
                noise.FrequencyGain = startNoise * 8;
            }
            if (PlayerController.Instance.transform.position.y < -300f)
            {
                noise.FrequencyGain = startNoise * 10;
            }
            if (PlayerController.Instance.transform.position.y < -323f)
            {
                noise.FrequencyGain = startNoise * 12;
            }
            if (PlayerController.Instance.transform.position.y < -359f)
            {
                noise.FrequencyGain = startNoise * 14;
            }
            if (PlayerController.Instance.transform.position.y < -400f)
            {
                noise.FrequencyGain = startNoise * 16;
            }
        }
    }
}
