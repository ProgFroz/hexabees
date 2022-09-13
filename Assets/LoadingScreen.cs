using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour {
    public Canvas loadingScreenCanvas;

    public NewMapMenu newMapMenu;
    public SaveLoadMenu saveLoadMenu;

    public Button newGameButton;
    public Button loadGameButton;
    
    // Start is called before the first frame update
    void Start()
    {
        newGameButton.onClick.AddListener((() => {
            newMapMenu.CreateMediumMap();
            Hide();
        }));
        
        loadGameButton.onClick.AddListener(() => {
            saveLoadMenu.Open(false);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hide() {
        this.loadingScreenCanvas.enabled = false;
    }
}
