using System;
using TMPro;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] private TMP_Text wordText;

    private string word;
    private int index;
    
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
            wordText.text = $"<color=yellow>{word[..index]}</color>{word[index..]}";

            if (index >= word.Length)
            {
                IsDone = true;
                gameObject.SetActive(false);
            }
            
            return;
        }

        index = 0;
        wordText.text = word;
    }
}