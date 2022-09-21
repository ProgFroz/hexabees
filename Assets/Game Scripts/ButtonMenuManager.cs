using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMenuManager : MonoBehaviour {

    public UIManager uiManager;
    public List<ButtonMenu> buttonMenus;
    public ButtonMenu activeButtonMenu;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateButtonMenu(ButtonMenu buttonMenu, bool isActive) {
        if (activeButtonMenu == null && !isActive) {
            buttonMenu.Toggle();
            this.activeButtonMenu = buttonMenu;
        }
        else {
            if (buttonMenu.transitioning || activeButtonMenu.transitioning) return;
            if (activeButtonMenu == buttonMenu && isActive) {
                buttonMenu.Toggle();
                DeactivateButtonMenu();
            }
            else if (activeButtonMenu != buttonMenu && !isActive) {
                activeButtonMenu.Toggle();
                buttonMenu.Toggle();
                this.activeButtonMenu = buttonMenu;
                this.uiManager.MenuClosed();
            }
        }
        
    }

    public void DeactivateButtonMenu() {
        this.activeButtonMenu = null;
        this.uiManager.MenuClosed();
    }
}
