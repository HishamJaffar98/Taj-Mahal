using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Narrator : MonoBehaviour
{
    GameObject audioManager;
    Slider musicSlider;
    float oldVolume;
    bool animationStarted = false;
    void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager");
        musicSlider = GameObject.FindGameObjectWithTag("MusicSlider").GetComponent<Slider>() ;
        musicSlider.value = 0.4f;
    }

    // Update is called once per frame
    void Update()
    {
        audioManager.GetComponent<AudioSource>().volume=musicSlider.value;
    }

    public void StartNarration()
    { 
        if(!animationStarted)
        {
            oldVolume = audioManager.GetComponent<AudioSource>().volume;
            audioManager.GetComponent<AudioSource>().volume *= 0.5f;
            musicSlider.value = audioManager.GetComponent<AudioSource>().volume;
        }
        animationStarted = true;
        GetComponent<AudioSource>().Play();
    }

    public void PauseNarration()
    {
        GetComponent<AudioSource>().Pause();
    }

    public void ResumeNarration()
    {
        GetComponent<AudioSource>().Play();
    }

    public void StopNarration()
    {
        GetComponent<AudioSource>().Stop();
        audioManager.GetComponent<AudioSource>().volume = oldVolume;
        musicSlider.value = audioManager.GetComponent<AudioSource>().volume;
        animationStarted = false;
    }
}
