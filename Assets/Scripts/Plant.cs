using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Plant : MonoBehaviour
{
    [SerializeField] private TMP_Text wordText;
    [SerializeField] private List<GameObject> leaves;
    [SerializeField] private List<SpriteRenderer> cobs;

    private string word;
    private int index;

    public Action<Plant> onDone;
    
    public bool IsDone { get; private set; }

    private void Start()
    {
        leaves.ForEach(l => l.SetActive(Random.value < 0.7f));
        cobs.ForEach(RandomizeCob);
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
    }

    public void Fill(string letter)
    {
        if (word.Substring(index, 1) == letter)
        {
            index++;
            wordText.text = $"<color=#EDD83D><size=6>{word[..index]}</size></color>{word[index..]}";

            if (index >= word.Length)
            {
                onDone?.Invoke(this);
                IsDone = true;
                Invoke(nameof(Remove), 0.5f);
            }
            
            return;
        }

        index = 0;
        wordText.text = word;
    }

    private void Remove()
    {
        gameObject.SetActive(false);
    }

    public int GetWordLength()
    {
        return word.Length;
    }
}