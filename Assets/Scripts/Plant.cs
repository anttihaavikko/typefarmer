using System;
using System.Collections.Generic;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Managers;
using TMPro;
using UnityEngine;
using AnttiStarterKit.Extensions;
using Random = UnityEngine.Random;

public class Plant : MonoBehaviour
{
    [SerializeField] private TMP_Text wordText;
    [SerializeField] private List<GameObject> leaves;
    [SerializeField] private List<SpriteRenderer> cobs;
    [SerializeField] private WobblingText wobble;
    [SerializeField] private Transform center;
    [SerializeField] private Shaker shaker;

    private string word;
    private int index;

    public Action<Plant> onDone;

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

    public void Fill(string letter)
    {
        if (word.Substring(index, 1) == letter)
        {
            index++;
            wordText.text = $"<color=#EDD83D>{word[..index]}</color>{word[index..]}";
            wobble.EndIndex = index;

            var offset = new Vector3(0.2f, -0.2f, 0);
            EffectManager.AddEffect(1, wordText.transform.position + wordText.textInfo.characterInfo[index - 1].topLeft + offset);

            if (index >= word.Length)
            {
                onDone?.Invoke(this);
                IsDone = true;
            }
            
            return;
        }

        if (index > 0)
        {
            shaker.Shake();
        }

        index = 0;
        wobble.EndIndex = index;
        wordText.text = word;
    }

    public void Remove()
    {
        EffectManager.AddEffects(new []{ 0, 2 }, center.position);
        
        Destroy(wordText.gameObject);
        Destroy(gameObject);
    }

    public int GetWordLength()
    {
        return word.Length;
    }
}