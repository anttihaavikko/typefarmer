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
        EffectManager.AddEffect(3, leftFoot.transform.position);
        leftFoot.Emit(new ParticleSystem.EmitParams(), 1);
    }

    public void StepRight()
    {
        EffectManager.AddEffect(3, rightFoot.transform.position);
        rightFoot.Emit(new ParticleSystem.EmitParams(), 1);
    }
}