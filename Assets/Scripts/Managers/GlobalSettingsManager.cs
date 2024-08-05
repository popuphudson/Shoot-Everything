using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettingsManager : MonoBehaviour
{
    public static GlobalSettingsManager Instance;
    public Vector2 MouseSensitivity = new Vector2(3, 3);
    public bool ShowHealthBars = false;
    public float FOV = 90;
    public float Sound = 0.5f;


    private void Awake() {
        if(GlobalSettingsManager.Instance) {
            Destroy(gameObject);
            return;
        }
        GlobalSettingsManager.Instance = this;
        DontDestroyOnLoad(gameObject);
        MouseSensitivity = new Vector2(PlayerPrefs.GetFloat("MouseX", 3), PlayerPrefs.GetFloat("MouseY", 3));
        ShowHealthBars = PlayerPrefs.GetInt("HealthBars", 1)==1;
        FOV = PlayerPrefs.GetFloat("FOV", 90f);
        Sound = PlayerPrefs.GetFloat("Sound", 0.5f);
    }

    private void OnApplicationQuit() {
        PlayerPrefs.SetFloat("MouseX", MouseSensitivity.x);
        PlayerPrefs.SetFloat("MouseY", MouseSensitivity.y);
        PlayerPrefs.SetInt("HealthBars", ShowHealthBars?1:0);
        PlayerPrefs.SetFloat("FOV", FOV);
        PlayerPrefs.SetFloat("Sound", Sound);
        PlayerPrefs.Save();
    }
}
