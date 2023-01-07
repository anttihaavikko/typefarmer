using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Game;
using AnttiStarterKit.Managers;
using TMPro;
using UnityEngine;

public class Plants : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Plant plantPrefab;
    [SerializeField] private WordDictionary words;
    [SerializeField] private TMP_Text plantCount;
    [SerializeField] private ScoreDisplay scoreDisplay;

    private readonly List<Plant> plants = new();

    private void Start()
    {
        words.Setup();
        SpawnPlant();
    }

    private void Update()
    {
        foreach (var letter in Input.inputString.Select(c => c.ToString().ToUpper()))
        {
            plants.ForEach(p => p.Fill(letter));
            scoreDisplay.DecreaseMulti();
        }
        
        plants.RemoveAll(p => p.IsDone);

        plantCount.text = $"PLANTS: {plants.Count}";
    }

    private void SpawnPlant()
    {
        var plant = Instantiate(plantPrefab, player.transform.position.RandomOffset(4f), Quaternion.identity);
        plant.Setup(words.GetRandomWord(6).ToUpper());
        plants.Add(plant);

        plant.onDone += MoveToPlant;
        
        Invoke(nameof(SpawnPlant), 2f);
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
        }, 0.3f);
    }
}