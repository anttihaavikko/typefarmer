using AnttiStarterKit.Animations;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Face face;
    
    public void LookAt(Vector3 pos)
    {
        face.LookTarget = pos;
    }
}