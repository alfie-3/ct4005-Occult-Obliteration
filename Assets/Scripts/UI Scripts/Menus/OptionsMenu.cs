using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {
    [SerializeField] public AudioMixer masterMixer;
    [SerializeField] public Slider musicSlider;
    [SerializeField] public Slider sfxSlider;
    [SerializeField] public Slider masterSlider;

    Resolution[] resolutions;
    Resolution selectedResolution;

    public TMP_Dropdown resolutionDropdown;
    public Toggle fullScreenToggle;

    private void Awake() {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++) {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height) {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void OnEnable() {
        SetAudioSliders();
    }

    public void SetAudioSliders() {
        masterSlider.value = Mathf.Exp(PlayerPrefs.GetFloat("MasterVolume", -40) / 20);
        sfxSlider.value = Mathf.Exp(PlayerPrefs.GetFloat("SFXVolume", -40) / 20);
        musicSlider.value = Mathf.Exp(PlayerPrefs.GetFloat("MusicVolume", -40) / 20);
    }


    public void SetMasterVolume(float setMasterVolume) {
        float logarithmicVolume = Mathf.Log(setMasterVolume) * 20;
        masterMixer.SetFloat("MasterVolume", logarithmicVolume);
        PlayerPrefs.SetFloat("MasterVolume", logarithmicVolume);
    }

    public void SetSfxVolume(float setSfxVolume) {
        float logarithmicVolume = Mathf.Log(setSfxVolume) * 20;
        masterMixer.SetFloat("SFXVolume", logarithmicVolume);
        PlayerPrefs.SetFloat("SFXVolume", logarithmicVolume);
    }
    public void SetMusicVolume(float setMusicVolume) {
        float logarithmicVolume = Mathf.Log(setMusicVolume) * 20;
        masterMixer.SetFloat("MusicVolume", logarithmicVolume);
        PlayerPrefs.SetFloat("MusicVolume", logarithmicVolume);
    }

    public void SetCurrentResolution(int resolutionIndex) {
        selectedResolution = resolutions[resolutionIndex];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
    }

    public void SetFullScreen(bool isFullScreen) {
        Screen.fullScreen = isFullScreen;
        PlayerPrefs.SetInt("IsFullscreen", isFullScreen ? 1 : 0);
    }
}
