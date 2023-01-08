using System;
using UnityEngine;

public class StartView : MonoBehaviour
{
    private bool canInteract;

    private void Start()
    {
        Invoke(nameof(EnableInteraction), 1f);
    }

    private void EnableInteraction()
    {
        canInteract = true;
    }

    private void Update()
    {
        if (!canInteract) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            return;
        }
        
        if (Input.anyKeyDown)
        {
            var scene = PlayerPrefs.HasKey("PlayerName") ? "Main" : "Name";
            SceneChanger.Instance.ChangeScene(scene);
        }
    }
}