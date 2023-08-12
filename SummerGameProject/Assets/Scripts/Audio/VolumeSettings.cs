using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider voiceVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;
    [SerializeField] Image muteVolumeButton;

    [SerializeField] Sprite mutedImage;
    [SerializeField] Sprite unmutedImage;

    float newVolume;
    float oldVolume;
    bool muted;

    private void Start()
    {
        muted = false;
        oldVolume = 0f;
        changeMasterVolume();
    }
    //functions for each slider :(
    #region
    public void changeMasterVolume ()
    {  
        newVolume = masterVolumeSlider.value;
        newVolume = Mathf.Log10(newVolume) * 20; // needed because sliders are liniar but volume is logarithmic
        audioMixer.SetFloat("Master", newVolume);
        if (muted)
        {
            muted = !muted;
            muteVolumeButton.sprite = unmutedImage;
        }
    }
    public void changeMusicVolume()
    {
        newVolume = musicVolumeSlider.value;
        newVolume = Mathf.Log10(newVolume) * 20; // needed because sliders are liniar but volume is logarithmic
        audioMixer.SetFloat("Music", newVolume);
        if (muted)
        {
            muted = !muted;
            muteVolumeButton.sprite = unmutedImage;
        }
    }
    public void changeVoiceVolume()
    {
        newVolume = voiceVolumeSlider.value;
        newVolume = Mathf.Log10(newVolume) * 20; // needed because sliders are liniar but volume is logarithmic
        audioMixer.SetFloat("Voice", newVolume);
        if (muted)
        {
            muted = !muted;
            muteVolumeButton.sprite = unmutedImage;
        }
    }
    public void changeSfxVolume()
    {
        newVolume = sfxVolumeSlider.value;
        newVolume = Mathf.Log10(newVolume) * 20; // needed because sliders are liniar but volume is logarithmic
        audioMixer.SetFloat("SFX", newVolume);
        if (muted)
        {
            muted = !muted;
            muteVolumeButton.sprite = unmutedImage;
        }
    }
    #endregion
    public void muteSwitch()
    {
        if (muted)
        {
            //change button sprite to mute sprite
            muteVolumeButton.sprite = unmutedImage;
            audioMixer.SetFloat("Master", oldVolume);
        }
        else
        {
            //change button sprite to unmute sprite
            muteVolumeButton.sprite = mutedImage;
            audioMixer.GetFloat("Master", out oldVolume);
            Debug.Log(oldVolume);
            audioMixer.SetFloat("Master", -80f);
        }
        muted = !muted;
    }
}
