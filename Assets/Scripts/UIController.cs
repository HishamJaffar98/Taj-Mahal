using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    bool musicOn;
    bool labelOn;
    bool autoRotationOn;
    bool faderActivated;
    bool animationPaused;
    bool UISwitched;
    bool animationStarted;
    bool buttonInteractable;
    bool labelShowing;
    float faderLerpFactor;
    float reverseFaderLerpFactor;
    float lerpIncrement;
    float labelLerpFactor;
    float labelLerpIncrement;
    float oldVolume;
    Color semiTransparent = new Color(1f, 1f, 1f, 0.45f);
    Color fullTransparent = new Color(1f, 1f, 1f, 0f);
    Color opaque = new Color(1f, 1f, 1f, 1f);
    GameObject[] labelObjects = new GameObject[5];
    List<GameObject> labelChildren = new List<GameObject>();

    //Cached Components
    AudioSource audioManager;
    Image faderPanel;
    AutoRotate autoRotator;
    Narrator narrator;

    //Serialfield GameObjects
    [SerializeField] GameObject animationStateText;
    [SerializeField] GameObject[] animationButtons;
    [SerializeField] Sprite[] animationButtonSprites;
    [SerializeField] Button[] topPanelButtons;
    void Start()
    {
        InitializeVariables();
        InitializeComponents();
        InitializeDataStructures();
    }

    void Update()
    {
        if (faderActivated)
        {
            ActivateFader();
        }
        CheckFaderOpacity();
        FadeLabel();
    }

    private void FadeLabel()
    {
        labelLerpFactor += labelLerpIncrement;
        if (!labelShowing)
        {
            foreach (GameObject label in labelChildren)
            {
                label.GetComponent<SpriteRenderer>().color = Color.Lerp(opaque, fullTransparent, labelLerpFactor);
            }
        }
        else
        {
            foreach (GameObject label in labelChildren)
            {
                label.GetComponent<SpriteRenderer>().color = Color.Lerp(fullTransparent, opaque, labelLerpFactor);
            }
        }
    }

    private void CheckFaderOpacity()
    {
        if (faderActivated && buttonInteractable)
        {
            ToggleButtonInteractability(false);
        }
        else if (!faderActivated && !buttonInteractable)
        {
            ToggleButtonInteractability(true);
        }
    }

    private void ToggleButtonInteractability(bool trigger)
    {
        foreach (GameObject button in animationButtons)
        {
            button.GetComponent<Button>().interactable = trigger;
        }
        buttonInteractable = trigger;
    }

    private void InitializeVariables()
    {
        musicOn = true;
        labelOn = true;
        autoRotationOn = true;
        faderActivated = false;
        animationPaused = false;
        UISwitched = false;
        animationStarted = false;
        buttonInteractable = true;
        labelShowing = true;
        faderLerpFactor = 0f;
        reverseFaderLerpFactor = 0f;
        lerpIncrement = 0.01f;
        labelLerpFactor = 0f;
        labelLerpIncrement = 0.025f;
    }

    private void InitializeComponents()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioSource>();
        faderPanel = GameObject.FindGameObjectWithTag("FaderPanel").GetComponent<Image>();
        autoRotator = FindObjectOfType<AutoRotate>();
        narrator = FindObjectOfType<Narrator>();
    }

    private void InitializeDataStructures()
    {
        labelObjects = GameObject.FindGameObjectsWithTag("Label");
        foreach(GameObject label in labelObjects)
        {
            for(int i=0;i<label.transform.childCount;i++)
            {
                labelChildren.Add(label.transform.GetChild(i).gameObject);
            }
        }
        
    }

    private void ActivateFader()
    {
        if (faderLerpFactor < 1f)
        {
            faderPanel.color = Color.Lerp(new Color(0f, 0f, 0f, 0f), new Color(0f, 0f, 0f, 1f), faderLerpFactor);
            faderLerpFactor += lerpIncrement;
        }
        else if (faderLerpFactor >= 1f)
        {
            autoRotator.gameObject.transform.eulerAngles= new Vector3(0f, 0f, 0f);
            if(animationStarted && !UISwitched)
            {
                autoRotator.gameObject.GetComponent<Animator>().SetBool("walkthroughStarted", true);
                UISwitched =SwitchAnimationUI(-21f, "Pause/Stop Animation",false);
            }
            else if(!animationStarted && UISwitched)
            {
                BringToOriginalState();
                UISwitched = SwitchAnimationUI(66f, "Start Animation", true);
            }
            faderPanel.color = Color.Lerp(new Color(0, 0, 0, 1), new Color(0, 0, 0, 0), reverseFaderLerpFactor);
            reverseFaderLerpFactor += lerpIncrement;
        }
        if (faderLerpFactor >= 1f && reverseFaderLerpFactor >= 1f)
        {
            faderLerpFactor = 0f;
            reverseFaderLerpFactor = 0f;
            faderActivated = false;
        }
    
    }

    private void BringToOriginalState()
    {
        if(animationPaused)
        {
            autoRotator.GetComponent<Animator>().enabled = true;
            TogglePauseUI("Pause/Stop Animation", 1);
            animationPaused = false;
        }
        autoRotator.gameObject.GetComponent<Animator>().SetBool("walkthroughStarted", false);
        autoRotator.AutoRotating = true;
        labelShowing = true;
        foreach (Button button in topPanelButtons)
        {
            button.interactable = true;
        }
    }

    private bool SwitchAnimationUI(float textYCoord,string stateText,bool buttonActiveState)
    {
        animationStateText.transform.localPosition = new Vector2(animationStateText.transform.localPosition.x, textYCoord);
        animationStateText.GetComponent<TextMeshProUGUI>().text = stateText;
        animationButtons[0].SetActive(buttonActiveState);
        animationButtons[1].SetActive(!buttonActiveState);
        animationButtons[2].SetActive(!buttonActiveState);
        return !buttonActiveState;
    }

    private bool ToggleSprite(bool toggleVariable)
    {
        if (toggleVariable)
        {
            toggleVariable = false;
            EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = semiTransparent;
        }
        else if (!toggleVariable)
        {
            toggleVariable = true;
            EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = opaque;
        }
        return toggleVariable;
    }

    private void TogglePauseUI(string stateText, int index)
    {
        animationStateText.GetComponent<TextMeshProUGUI>().text = stateText;
        animationButtons[1].GetComponent<Image>().sprite = animationButtonSprites[index];
    }

    public void ToggleMusic()
    {
        if (musicOn)
        {
            audioManager.Pause();
        }
        else
        {
            audioManager.Play();
        }
        musicOn = ToggleSprite(musicOn);
    }
    public void ToggleLabel()
    {
        labelOn = ToggleSprite(labelOn);
        if (labelShowing)
        {
            labelShowing = false;
        }
        else
        {
            labelShowing = true;
        }
        labelLerpFactor = 0f;
    }
    public void ToggleRotation()
    {
        if(autoRotator.AutoRotating)
        {
            autoRotator.AutoRotating = false;
        }
        else
        {
            autoRotator.AutoRotating = true;
        }
        autoRotationOn=ToggleSprite(autoRotationOn);
    }

    public void ExitApplication()
    {
        StartCoroutine(DelayAndQuit());   
    }

    IEnumerator DelayAndQuit()
    {
        EventSystem.current.currentSelectedGameObject.gameObject.GetComponent<Button>().interactable = false;
        yield return new WaitForSecondsRealtime(0.5f);
        Application.Quit();
    }
    public void StartAnimation()
    {
        oldVolume = audioManager.volume;
        animationStarted = true;
        autoRotator.AutoRotating = false;
        labelShowing = false;
        faderActivated = true;
        foreach(Button button in topPanelButtons)
        {
            button.interactable = false;
        }
    }

    public void PauseAnimation()
    {
        if(!animationPaused)
        {
            autoRotator.GetComponent<Animator>().enabled = false;
            narrator.PauseNarration();
            animationPaused = true;
            TogglePauseUI("Play/Stop Animation",0);
        }
        else
        {
            autoRotator.GetComponent<Animator>().enabled = true;
            narrator.ResumeNarration();
            animationPaused = false;
            TogglePauseUI("Pause/Stop Animation", 1);
        }
        
    }

    public void StopAnimation()
    {
        audioManager.volume = oldVolume;
        animationStarted = false;
        narrator.StopNarration();
        faderActivated = true;
    }


}
