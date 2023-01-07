using System;
using TMPro;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] private TMP_Text wordText;

    private string word;
    private int index;

    public Action<Plant> onDone;
    
    public bool IsDone { get; private set; }
    
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
            wordText.text = $"<color=yellow><size=6>{word[..index]}</size></color>{word[index..]}";

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
}