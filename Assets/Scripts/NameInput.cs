using System;
using System.Collections;
using System.Collections.Generic;
using AnttiStarterKit.Managers;
using AnttiStarterKit.ScriptableObjects;
using AnttiStarterKit.Visuals;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class NameInput : MonoBehaviour
{
    public TMP_InputField field;
    public EffectCamera cam;
    [SerializeField] private SoundCollection picks, bass;

    private void Start()
    {
        field.onValueChanged.AddListener(ToUpper);
        Invoke(nameof(FocusInput), 0.6f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            Save();
        }
    }

    private void FocusInput()
	{
        EventSystem.current.SetSelectedGameObject(field.gameObject, null);
        field.OnPointerClick(new PointerEventData(EventSystem.current));
    }

    private void ToUpper(string value)
    {
        field.text = value;
    }

    public void Save()
    {
        if (string.IsNullOrEmpty(field.text))
        {
            AudioManager.Instance.PlayEffectAt(bass.At(0), Vector3.zero, 0.6f, false);
            Invoke(nameof(FocusInput), 0.1f);
            return;
        }
        
        AudioManager.Instance.PlayEffectAt(1, Vector3.zero, 0.5f, false);
        
        PlayerPrefs.SetString("PlayerName", field.text);
        PlayerPrefs.SetString("PlayerId", Guid.NewGuid().ToString());
        SceneChanger.Instance.ChangeScene("Main");
    }

    public void LostFocus()
    {
        Invoke(nameof(FocusInput), 0.1f);
    }

    public void OnChange()
    {
        AudioManager.Instance.PlayEffectFromCollection(1, Vector3.zero);
        AudioManager.Instance.PlayEffectFromCollection(2, Vector3.zero);
        AudioManager.Instance.PlayEffectAt(picks.At(field.text.Length % 7), Vector3.zero, 0.2f, false);
    }
}
