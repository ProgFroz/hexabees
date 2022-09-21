using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMenu : MonoBehaviour
{
    public UIManager uiManager;
    public Button baseButton;
    public Button innerBaseButton;
    public List<BeeAction> currentActions;
    public List<Button> buttons = new List<Button>();
    // public List<GameObject> basePivots;
    public List<GameObject> buttonPivots;
    public GameObject pivotInactive;
    public GameObject pivotActive;
    UnityEngine.UI.Outline outline;
    public bool isActive;
    public bool transitioning;
    private float BASE_BUTTON_ACTIVE_POS_Y = 68f;
    private float BASE_BUTTON_INACTIVE_POS_Y = 0;
    public float BASE_BUTTON_TRANSITION_TIME = 0.1f;
    public float BUTTONS_TRANSITION_TOGGLE_TIME = 0.1f;
    public float BUTTONS_TRANSITION_SHOW_TIME = 0.2f;
    public float BUTTONS_TRANSITION_DELAY = 0.1f;
    private readonly Vector3 BUTTONS_SCALE = new Vector3(1, 1, 1);
    public Vector3 BASE_BUTTON_SCALE = new Vector3(0.15f, 0.15f, 0.15f);
    private bool transitioningBaseButton;

    public ButtonMenuManager buttonMenuManager;
    void Start()
    {
        this.BuildMenu();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (uiManager.TimeManager.gameIsPaused) return;

    }
    
    public void BuildMenu()
    {
        outline = baseButton.GetComponent<UnityEngine.UI.Outline>();
        if (currentActions != null)
        {
            if (baseButton == null) throw new System.Exception("Base Button not assigned in " + gameObject.name);
            this.buttons = new List<Button>();
            
            foreach (Button button in uiManager.allActionButtons)
            {
                ActionButton actionButton = button.GetComponent<ActionButton>();
                
                if (currentActions.Contains(actionButton.beeAction))
                {
                    this.buttons.Add(button);

                }
                
            }
            for (int i = 0; i < Mathf.Min(buttonPivots.Count, buttons.Count); i++)
            {
                Transform pivotTransform = buttonPivots[i].transform;
                Button button = buttons[i];
                button.transform.position = pivotTransform.position;
            }
            if (this.currentActions != null && this.isActive)
            {
                this.ToggleShowButtons(true);
            }
        }
        baseButton.onClick.AddListener(() => this.buttonMenuManager.UpdateButtonMenu(this, isActive));
        innerBaseButton.onClick.AddListener(() => this.buttonMenuManager.UpdateButtonMenu(this, isActive));
    }

    public void Toggle()
    {
        
        if (!transitioning)
        {
            this.transitioning = true;
            if (isActive)
            {
                this.ToggleButtonsReverse();
            }
            else
            {
                this.ToggleBaseButton();
            }
        }
        
    }

    public void Toggle(bool force) {
        if (force && !this.isActive) {
            this.ToggleBaseButton();
        }

        if (!force && this.isActive) {
            this.ToggleButtonsReverse();
        }
    }

    private void ToggleButtons()
    {
        if (this.buttons == null) return;
        LTSeq seq = LeanTween.sequence();
        foreach (Button button in buttons)
        {
            seq.append(LeanTween.scale(button.gameObject, this.isActive ? Vector3.zero : BUTTONS_SCALE, BUTTONS_TRANSITION_TOGGLE_TIME)
                .setDelay(BUTTONS_TRANSITION_DELAY));
        }
        seq.append(LeanTween.scale(baseButton.gameObject, baseButton.transform.localScale, 0.001f).setOnComplete(() =>
        {
            this.transitioning = false;
            //outline.enabled = true;
        }
        ));
        this.isActive = !this.isActive;
    }
    private void ToggleButtonsReverse()
    {
        if (this.buttons == null) return;
        LTSeq seq = LeanTween.sequence();
        List<Button> reversedButtons = new List<Button>(this.buttons);
        reversedButtons.Reverse();
        foreach (Button button in reversedButtons)
        {
            seq.append(LeanTween.scale(button.gameObject, this.isActive ? Vector3.zero : BUTTONS_SCALE, BUTTONS_TRANSITION_TOGGLE_TIME)
                .setDelay(BUTTONS_TRANSITION_DELAY));
        }
        seq.append(LeanTween.scale(baseButton.gameObject, baseButton.transform.localScale, 0.001f).setOnComplete(() =>
        this.ToggleBaseButtonReverse()));
        this.isActive = !this.isActive;
    }

    private void ToggleBaseButton()
    {
        bool isActive = this.isActive;
        RectTransform rect = baseButton.GetComponent<RectTransform>();
        LeanTween.value(baseButton.gameObject, rect.anchoredPosition, new Vector2(rect.localPosition.x, isActive ?
            BASE_BUTTON_INACTIVE_POS_Y : BASE_BUTTON_ACTIVE_POS_Y), BASE_BUTTON_TRANSITION_TIME).setOnUpdate(
            (Vector2 val) => {
                rect.anchoredPosition = val;
            }
        ).setEaseInOutCubic().setOnComplete(() => this.ToggleButtons());
    }
    private void ToggleBaseButtonReverse()
    {
        bool isActive = this.isActive;
        RectTransform rect = baseButton.GetComponent<RectTransform>();
        LeanTween.value(baseButton.gameObject, rect.anchoredPosition, new Vector2(rect.localPosition.x, !isActive ?
            BASE_BUTTON_INACTIVE_POS_Y : BASE_BUTTON_ACTIVE_POS_Y), BASE_BUTTON_TRANSITION_TIME).setOnUpdate(
            (Vector2 val) => {
                rect.anchoredPosition = val;
            }
        ).setEaseInOutCubic().setOnComplete(() => { 
            this.transitioning = false; 
            //outline.enabled = false; 
        });
    }

    private void ToggleShowMenu(bool show)
    {
        if (this.baseButton.transform.localScale.Equals(Vector3.zero) && show)
        {
            if (!transitioningBaseButton)
            {
                transitioningBaseButton = true;
                LeanTween.scale(this.baseButton.gameObject, BASE_BUTTON_SCALE, BUTTONS_TRANSITION_TOGGLE_TIME).setEaseInOutCubic().setOnComplete(() => transitioningBaseButton = false);
                if (this.isActive)
                {
                    ToggleShowButtons(show);
                }
            }
        }
        else if (this.baseButton.transform.localScale.Equals(BASE_BUTTON_SCALE) && !show)
        {
            if (!transitioningBaseButton)
            {
                transitioningBaseButton = true;
                LeanTween.scale(this.baseButton.gameObject, Vector3.zero, BUTTONS_TRANSITION_TOGGLE_TIME).setEaseInOutCubic().setOnComplete(() => transitioningBaseButton = false);
                ToggleShowButtons(show);
            }
        }
    }
    private void ToggleShowButtons(bool show)
    {
        foreach (Button button in buttons)
        {
            
            // LeanTween.scale(button.gameObject, show ? BUTTONS_SCALE : Vector3.zero, BUTTONS_TRANSITION_TOGGLE_TIME).setEaseInOutCubic();
        }
    }

    private void ToggleShowButtonsUpDown()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            Button button = buttons[i];
            LTDescr desc = LeanTween.scale(button.gameObject, Vector3.zero, BUTTONS_TRANSITION_TOGGLE_TIME).setEaseInOutCubic();
            if (i == 0)
            {
                desc.setOnComplete(() => ToggleShowButtons(true));
            }
        }
    }
}

public enum BeeAction {
    Gather,
    Pollinate,
    Fertilize,
    Lay,
    Cancel,
    Destroy,
    
    Evaporator,
    Mixer,
    Refiner,
    Storage,
    Breed,
    Royal,
    
    None,
    
    // Nurse
    Feed,
    Cover,
    
    // All
    Eat,
    Drink,
    
    // Builder
    Mix,
    Refine,
    Evaporate,
    
    Store
}