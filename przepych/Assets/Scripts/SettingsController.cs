using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{
    public AudioMixer audioMixer;
    public AudioMixer sfxAudioMixer;

    public AudioSource audioSource;

    public Slider slider;
    public Slider sfxSlider;

    public TMP_Dropdown resolutionDropdown;

    Resolution[] resolutions;

    public void Start()
    { 
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for(int i=0;i<resolutions.Length;i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height}";
            options.Add(option);

            if (resolutions[i].width==Screen.currentResolution.width && resolutions[i].height==Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        //options.Add("");

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        //if (resolutions[resolutionIndex].ToString()!="")
        //{
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        //}

    }

    public void SetVolume(float volume)
    { 
        if (slider.value == slider.minValue)
        {
            audioMixer.SetFloat("Volume", -80);
        }
        else
        {
            audioMixer.SetFloat("Volume", volume);
        }
    }
    public void SetSfxVolume(float volume)
    {  
        if (sfxSlider.value == sfxSlider.minValue)
        {
            sfxAudioMixer.SetFloat("Volume", -80);
        }
        else
        {
            sfxAudioMixer.SetFloat("Volume", volume);
        }
    }

    public void SetFullScreen (bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
}
