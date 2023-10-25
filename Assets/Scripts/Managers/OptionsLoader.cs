using UnityEngine;
using UnityEngine.Audio;

public class OptionsLoader : MonoBehaviour {
    [SerializeField] AudioMixer mixer;

    private void Start() {
        LoadSoundSettings();
    }

    private void LoadSoundSettings() {
        if (PlayerPrefs.GetFloat("MasterVolume", -40) != float.NaN) {
            mixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume", -40));
        }
        else {
            mixer.SetFloat("MasterVolume", 0);
        }

        if (PlayerPrefs.GetFloat("SFXVolume", -40) != float.NaN) {
            mixer.SetFloat("SFXVolume", PlayerPrefs.GetFloat("SFXVolume", -40));
        }
        else {
            mixer.SetFloat("SFXVolume", 0);
        }

        if (PlayerPrefs.GetFloat("MusicVolume", -40) != float.NaN) {
            mixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume", -40));
        }
        else {
            mixer.SetFloat("MusicVolume", 0);
        }

    }
}
