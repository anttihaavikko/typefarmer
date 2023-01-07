using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Game;
using AnttiStarterKit.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Plants : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Plant plantPrefab;
    [SerializeField] private WordDictionary words;
    [SerializeField] private ScoreDisplay scoreDisplay;
    [SerializeField] private Image overgrowthBar;
    [SerializeField] private GameObject gameOverDisplay;
    [SerializeField] private Color red, yellow;
    [SerializeField] private Shaker barShaker;
    [SerializeField] private GameObject zoomCamera;

    private readonly List<Plant> plants = new();
    private bool ended;

    private void Start()
    {
        words.Setup();
        SpawnPlant();
    }

    private void Update()
    {
        if (ended) return;
        
        foreach (var letter in Input.inputString.Select(c => c.ToString().ToUpper()))
        {
            plants.ForEach(p => p.Fill(letter));
            scoreDisplay.DecreaseMulti();
        }
        
        plants.RemoveAll(p => p.IsDone);
    }

    private void UpdateOvergrowth()
    {
        var size = new Vector3(Mathf.Clamp01(plants.Count(p => !p.IsDone) / 10f), 1f, 1f);
        Tweener.ScaleToBounceOut(overgrowthBar.transform, size, 0.2f);
        overgrowthBar.color = GetBarColor();

        if (plants.Count(p => !p.IsDone) >= 10)
        {
            barShaker.ShakeForever();
            return;
        }

        barShaker.StopShaking();
    }

    private Color GetBarColor()
    {
        var count = plants.Count(p => !p.IsDone);
        if (count >= 10) return red;
        if (count >= 9) return yellow;
        return Color.white;
    }

    private void SpawnPlant()
    {
        if (ended) return;
        
        var plant = Instantiate(plantPrefab, FindSpot(), Quaternion.identity);
        plant.Setup(words.GetRandomWord(6).ToUpper());
        plants.Add(plant);

        plant.onDone += MoveToPlant;
        
        UpdateLook();
        UpdateOvergrowth();

        if (plants.Count(p => !p.IsDone) >= 11)
        {
            gameOverDisplay.SetActive(true);
            ended = true;
            barShaker.StopShaking();
            barShaker.gameObject.SetActive(false);
            return;
        }
        
        Invoke(nameof(SpawnPlant), 2f);
    }

    private Vector3 FindSpot()
    {
        var tries = 0;
        var playerPos = player.transform.position;
        var occupied = plants.Where(p => !p.IsDone).Select(p => p.transform.position).ToList();
        occupied.Add(playerPos);

        while (true)
        {
            var p = playerPos.RandomOffset(4f);
            if (tries > 100 || occupied.All(o => Vector3.Distance(o, p) > 2f)) return p;
            tries++;
        }
    }

    private void MoveToPlant(Plant plant)
    {
        var length = plant.GetWordLength();
        plant.onDone -= MoveToPlant;
        Tweener.MoveToBounceOut(player.transform, plant.transform.position, 0.3f);
        this.StartCoroutine(() =>
        {
            scoreDisplay.Add(length);
            scoreDisplay.AddMulti(length);
            UpdateLook();
            UpdateOvergrowth();
        }, 0.3f);
    }

    private void UpdateLook()
    {
        if (!plants.Any()) return;
        var playerPos = player.transform.position;
        var closest = plants.Where(p => !p.IsDone).OrderBy(p => Vector3.Distance(p.transform.position, playerPos)).First();
        player.LookAt(closest.transform.position);
    }
}