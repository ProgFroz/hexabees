using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour {
    public Canvas loadingScreenCanvas;

    [SerializeField] private TimeManager timeManager;
    public NewMapMenu newMapMenu;
    public SaveLoadMenu saveLoadMenu;

    public Button newGameButton;
    public Button loadGameButton;
    
    // Start is called before the first frame update
    void Start()
    {
        newGameButton.onClick.AddListener((CreateNewGame));
        
        loadGameButton.onClick.AddListener(() => {
            saveLoadMenu.Open(false);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateNewGame() {
        newMapMenu.CreateMediumMap();
        
        Hide();
    }

    public void Hide() {
        this.loadingScreenCanvas.enabled = false;
    }
}
