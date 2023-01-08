using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class CustomCaret : MonoBehaviour
{
    [SerializeField] private TMP_Text field;
    
    private void Update()
    {
        field.ForceMeshUpdate();
        transform.position = field.transform.position + field.textInfo.characterInfo.Last().topRight;
    }
}