using System;
using UnityEngine;

public class FollowLocalPosition : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 scale = Vector3.one;

    private void Update()
    {
        var p = target.localPosition;
        transform.localPosition = new Vector3(p.x * scale.x, p.y * scale.y, p.z * scale.z);
    }
}