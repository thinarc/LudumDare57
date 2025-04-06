using UnityEngine;

public class NoodleAnimator : MonoBehaviour
{
    private Animator anim;
    private INoodleController noodle;

    private AudioSource sc;

    [SerializeField] private AudioClip noise;
    [SerializeField] private AudioClip catche;
    [SerializeField] private AudioClip fade;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        noodle = GetComponentInParent<INoodleController>();

        sc = GetComponent<AudioSource>();
        sc.volume *= PlayerPrefs.GetFloat("VFX");
    }

    private void OnEnable()
    {
        noodle.LeaveGround += OnLeaveGround;
        noodle.Catch += OnCatch;
    }

    private void OnDisable()
    {
        noodle.LeaveGround -= OnLeaveGround;
        noodle.Catch -= OnCatch;
    }

    private void Update()
    {
        HandleMove();
    }

    private bool walked;
    private void HandleMove()
    {
        var inputStrength = Mathf.Abs(noodle.FrameDirection.x);

        if (!noodle.ViewTarget)
        {
            anim.SetBool(RunKey, inputStrength > 0);
            anim.SetBool(WalkKey, false);
        }
        else
        {
            anim.SetBool(WalkKey, inputStrength > 0);
            anim.SetBool(RunKey, false);
        }

        if (anim.GetBool(RunKey) || anim.GetBool(WalkKey))
        {
            if (walked) return;
            sc.clip = noise;
            sc.Play();
            walked = true;
        }
    }

    private void OnLeaveGround()
    {
        anim.SetTrigger(FadeAwayKey);
        if (sc.volume != 0) sc.volume += 1f;
        if (fade) sc.PlayOneShot(fade);
        sc.Stop();
    }

    private void OnCatch()
    {
        anim.SetTrigger(CatchKey);
        if (sc.volume != 0) sc.volume += 0.1f;
        if (catche) sc.PlayOneShot(catche);
        // sc.Stop();
    }

    public void CleanDestroy()
    {
        Destroy(transform.parent.gameObject);
    }

    private static readonly int RunKey = Animator.StringToHash("Run");
    private static readonly int WalkKey = Animator.StringToHash("Walk");
    private static readonly int FadeAwayKey = Animator.StringToHash("FadeAway");
    private static readonly int CatchKey = Animator.StringToHash("Catch");
}
