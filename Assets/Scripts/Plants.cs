using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Game;
using AnttiStarterKit.Managers;
using AnttiStarterKit.Visuals;
using Leaderboards;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    [SerializeField] private EffectCamera cam;
    [SerializeField] private ScoreManager scoreManager;

    private readonly List<Plant> plants = new();
    private bool ended;
    private readonly Queue<Plant> moveQueue = new();
    private bool moving;
    private int maxLength = 4;
    private bool endKeysEnabled;
    private bool filled = true;
    private float spawnDelay = 2.5f;
    private int matches;

    private const float WalkDuration = 0.2f;

    private void Start()
    {
        words.Setup();
        SpawnPlant(true);
        filled = false;
    }

    private void Update()
    {
        if (endKeysEnabled)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
                return;
            }

            if (Input.anyKeyDown)
            {
                SceneChanger.Instance.ChangeScene("Main");
                return;
            }
        }
        
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

    private void SpawnPlantWithTimer()
    {
        SpawnPlant();
    }

    private void SpawnPlant(bool first = false)
    {
        if (!filled || ended) return;
        
        var plant = Instantiate(plantPrefab, FindSpot(), Quaternion.identity);
        var min = Mathf.Max(3, Mathf.CeilToInt(maxLength * 0.5f));
        plant.Setup(words.GetRandomWord(Random.Range(min, maxLength + 1)).ToUpper());
        plants.Add(plant);

        plant.onDone += MoveToPlant;
        
        UpdateLook();
        UpdateOvergrowth();

        if (plants.Count(p => !p.IsDone) >= 11)
        {
            cam.BaseEffect(0.5f);
            player.Lose();
            gameOverDisplay.SetActive(true);
            ended = true;
            barShaker.StopShaking();
            barShaker.gameObject.SetActive(false);
            Invoke(nameof(EnableKeys), 1f);
            return;
        }

        if (!first)
        {
            Invoke(nameof(SpawnPlantWithTimer), spawnDelay);   
        }

        spawnDelay *= 0.995f;
        Debug.Log($"Spawn delay now {spawnDelay}");
    }

    private void EnableKeys()
    {
        var plr = PlayerPrefs.GetString("PlayerName", "Anon");
        var id = PlayerPrefs.GetString("PlayerId", "000-000-000");
        scoreManager.SubmitScore(plr, scoreDisplay.Total, matches, id);
        
        endKeysEnabled = true;
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

    private Vector3 GetTextOffset()
    {
        const float amount = 0.7f;
        return Vector3.zero.WhereX(Random.Range(-amount, amount));
    }

    private void MoveToPlant(Plant plant)
    {
        if (moving)
        {
            moveQueue.Enqueue(plant);
            return;
        }
        
        var plantPos = plant.transform.position;
        var distance = Vector3.Distance(player.transform.position, plantPos);
        var duration = distance * WalkDuration;
        var length = plant.GetWordLength();
        plant.onDone -= MoveToPlant;
        Tweener.MoveTo(player.transform, plantPos, duration, TweenEasings.SineEaseInOut);
        player.Run(duration - 0.1f);

        player.LookAt(plantPos);
        
        moving = true;
        
        cam.BaseEffect(0.1f);

        this.StartCoroutine(() => player.Swing(), duration * 0.8f);
        
        this.StartCoroutine(() =>
        {
            cam.BaseEffect(0.2f);
            
            var score = Mathf.CeilToInt(length * length * distance);
            UpdateOvergrowth();
            plant.Remove();
            moving = false;
            
            UpdateLook();

            EffectManager.AddTextPopup((score * scoreDisplay.Multi).AsScore(), plantPos + Vector3.up * 2 + GetTextOffset());

            matches++;

            this.StartCoroutine(() =>
            {
                var bonusText =
                    $"<size=3>DISTANCE BONUS </size><size=5><color=#EDD83D>x{Mathf.CeilToInt(distance)}</color></size>";
                EffectManager.AddTextPopup(bonusText, plantPos + Vector3.up * 1.5f + GetTextOffset());
            }, 0.15f);

            if (plants.Count(p => !p.IsDone) == 0)
            {
                this.StartCoroutine(() =>
                {
                    var fullClearMessage = "<size=2.5>FULL CLEAR BONUS </size><size=4.5><color=#EDD83D>x2</color></size>";
                    EffectManager.AddTextPopup(fullClearMessage, plantPos + Vector3.up * 1f + GetTextOffset());
                }, 0.22f);
                score *= 2;
            }

            if (!ended)
            {
                scoreDisplay.Add(score);
                scoreDisplay.AddMulti(length);   
            }

            if (length == maxLength)
            {
                maxLength++;
            }

            if (moveQueue.Any())
            {
                MoveToPlant(moveQueue.Dequeue());
            }

            if (!filled)
            {
                filled = true;
                SpawnPlant();
            }

        }, duration);
    }

    private void UpdateLook()
    {
        if (moving) return;
        if (!plants.Any()) return;
        var playerPos = player.transform.position;
        var closest = plants.Where(p => !p.IsDone).OrderBy(p => Vector3.Distance(p.transform.position, playerPos)).First();
        player.LookAt(closest.transform.position);
    }
}