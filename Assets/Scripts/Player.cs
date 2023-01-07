using AnttiStarterKit.Animations;
using UnityEngine;
using AnttiStarterKit.Extensions;

public class Player : MonoBehaviour
{
    [SerializeField] private Face face;
    [SerializeField] private Animator anim;
    private static readonly int Running = Animator.StringToHash("running");

    public void LookAt(Vector3 pos)
    {
        face.LookTarget = pos;
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
}