using System;
using System.Collections.Generic;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

public class Demo : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private List<Transform> spots;

    private void Start()
    {
        Invoke(nameof(Move), Random.Range(1f, 5f));
    }

    private void Move()
    {
        var pos = spots.Random().position;
        var pt = player.transform;
        var distance = Vector3.Distance(pt.position, pos);
        var duration = distance * Plants.WalkDuration * 1.3f;
        Tweener.MoveTo(pt, pos, duration, TweenEasings.SineEaseInOut);
        player.Run(duration - 0.1f);
        
        Invoke(nameof(Move), Random.Range(1f, 5f));
    }
}