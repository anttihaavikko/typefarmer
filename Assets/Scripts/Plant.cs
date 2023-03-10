using System;
using System.Collections.Generic;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Managers;
using TMPro;
using UnityEngine;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.ScriptableObjects;
using Random = UnityEngine.Random;

public class Plant : MonoBehaviour
{
    [SerializeField] private TMP_Text wordText;
    [SerializeField] private List<GameObject> leaves;
    [SerializeField] private List<SpriteRenderer> cobs;
    [SerializeField] private WobblingText wobble;
    [SerializeField] private Transform center;
    [SerializeField] private Shaker shaker;
    [SerializeField] private SoundCollection bass, picks;

    private string word;
    private int index;

    public Action<Plant> onDone;

    public int Index => index;

    public bool IsDone { get; private set; }

    private void Start()
    {
        leaves.ForEach(l => l.SetActive(Random.value < 0.7f));
        cobs.ForEach(RandomizeCob);
        
        wobble.EndIndex = 0;

        EffectManager.AddEffect(0, center.position);
    }

    private void RandomizeCob(SpriteRenderer cob)
    {
        cob.gameObject.SetActive(Random.value < 0.7f);
        cob.flipX = Random.value < 0.5f;
        cob.flipY = Random.value < 0.5f;
    }

    public void Setup(string w)
    {
        word = w;
        wordText.text = word;
        
        wordText.transform.SetParent(null);
    }

    public void Fill(string letter, int bestIndex)
    {
        var isBest = index == bestIndex;
        
        if (word.Substring(index, 1) == letter)
        {
            index++;
            wordText.text = $"<color=#EDD83D>{word[..index]}</color>{word[index..]}";
            wobble.EndIndex = index;

            var offset = new Vector3(0.2f, -0.2f, 0);
            var p = wordText.transform.position + wordText.textInfo.characterInfo[index - 1].topLeft + offset;
            EffectManager.AddEffect(1, p);
            AudioManager.Instance.PlayEffectFromCollection(1, p);
            AudioManager.Instance.PlayEffectFromCollection(2, p);
            
            AudioManager.Instance.PlayEffectAt(picks.At((index - 1) % 7), p, 0.2f, false);

            if (index >= word.Length)
            {
                AudioManager.Instance.PlayEffectAt(1, p, 0.5f, false);
                
                onDone?.Invoke(this);
                IsDone = true;
            }
            
            return;
        }

        if (index > 0)
        {
            if (isBest)
            {
                var offset = new Vector3(0.2f, -0.2f, 0);
                var p = wordText.transform.position + wordText.textInfo.characterInfo[index - 1].topLeft + offset;
                AudioManager.Instance.PlayEffectAt(bass.At((index - 1) % 7), p, 1.3f, false);     
            }
            
            shaker.Shake();
        }

        index = 0;
        wobble.EndIndex = index;
        wordText.text = word;
    }

    public void Remove()
    {
        var p = center.position;
        AudioManager.Instance.PlayEffectFromCollection(3, p);
        EffectManager.AddEffects(new []{ 0, 2, 4 }, p);
        
        Destroy(wordText.gameObject);
        Destroy(gameObject);
    }

    public int GetWordLength()
    {
        return word.Length;
    }
}