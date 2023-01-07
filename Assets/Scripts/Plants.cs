using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Managers;
using UnityEngine;

public class Plants : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Plant plantPrefab;
    [SerializeField] private WordDictionary words;

    private readonly List<Plant> plants = new();

    private void Start()
    {
        words.Setup();
        SpawnPlant();
    }

    private void Update ()
    {
        foreach (var letter in Input.inputString.Select(c => c.ToString().ToUpper()))
        {
            plants.ForEach(p => p.Fill(letter));
        }
        
        plants.RemoveAll(p => p.IsDone);
    }

    private void SpawnPlant()
    {
        var plant = Instantiate(plantPrefab, Vector3.zero.RandomOffset(5f), Quaternion.identity);
        plant.Setup(words.GetRandomWord(6).ToUpper());
        plants.Add(plant);
        
        Invoke(nameof(SpawnPlant), 2f);
    }
}