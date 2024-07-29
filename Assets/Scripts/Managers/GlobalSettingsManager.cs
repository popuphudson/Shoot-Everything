using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettingsManager : MonoBehaviour
{
    public static GlobalSettingsManager Instance;
    public Vector2 MouseSensitivity = new Vector2(3, 3);
    public bool ShowHealthBars = false;
    public float FOV = 90;


    private void Awake() {
        if(GlobalSettingsManager.Instance) {
            Destroy(gameObject);
            return;
        }
        GlobalSettingsManager.Instance = this;
        DontDestroyOnLoad(gameObject);
        if(PlayerPrefs.GetInt("SAVED") == 0) return;
        MouseSensitivity = new Vector2(PlayerPrefs.GetFloat("MouseX"), PlayerPrefs.GetFloat("MouseY"));
        ShowHealthBars = PlayerPrefs.GetInt("HealthBars")==1;
        FOV = PlayerPrefs.GetFloat("FOV");
    }

    private void OnApplicationQuit() {
        PlayerPrefs.SetInt("SAVED", 1);
        PlayerPrefs.SetFloat("MouseX", MouseSensitivity.x);
        PlayerPrefs.SetFloat("MouseY", MouseSensitivity.y);
        PlayerPrefs.SetInt("HealthBars", ShowHealthBars?1:0);
        PlayerPrefs.SetFloat("FOV", FOV);
        PlayerPrefs.Save();
    }
}
