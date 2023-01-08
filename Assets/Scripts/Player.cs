using AnttiStarterKit.Animations;
using UnityEngine;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Managers;
using AnttiStarterKit.Visuals;

public class Player : MonoBehaviour
{
    [SerializeField] private Face face;
    [SerializeField] private Animator anim;
    [SerializeField] private EffectCamera cam;
    [SerializeField] private ParticleSystem leftFoot, rightFoot;

    private static readonly int Running = Animator.StringToHash("running");
    private static readonly int SwingAnim = Animator.StringToHash("swing");
    private static readonly int Lost = Animator.StringToHash("lost");

    public void LookAt(Vector3 pos)
    {
        face.LookTarget = pos;
    }

    public void Swing()
    {
        AudioManager.Instance.PlayEffectFromCollection(4, transform.position);
        anim.SetTrigger(SwingAnim);
    }

    public void Run(float duration)
    {
        anim.SetBool(Running, true);
        CancelInvoke(nameof(StopRunning));
        Invoke(nameof(StopRunning), duration);
    }

    private void StopRunning()
    {
        anim.SetBool(Running, false);
    }

    public void Lose()
    {
        face.Madden();
        anim.SetBool(Lost, true);
    }

    public void Stomp()
    {
        // cam.BaseEffect(0.15f);
    }

    public void StepLeft()
    {
        var p = leftFoot.transform.position;
        EffectManager.AddEffect(3, p);
        leftFoot.Emit(new ParticleSystem.EmitParams(), 1);
        AudioManager.Instance.PlayEffectFromCollection(0, p);
    }

    public void StepRight()
    {
        var p = rightFoot.transform.position;
        EffectManager.AddEffect(3, p);
        rightFoot.Emit(new ParticleSystem.EmitParams(), 1);
        AudioManager.Instance.PlayEffectFromCollection(0, p);
    }
}