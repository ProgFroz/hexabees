using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PriorityButton : MonoBehaviour {

    [SerializeField] private Button button;
    [SerializeField] private RawImage backgroundImage;
    [SerializeField] private RawImage highPriorityImage;
    [SerializeField] private RawImage mediumPriorityImage;
    [SerializeField] private RawImage lowPriorityImage;
    private RawImage _activeImage = null;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateImages(PriorityValue priority) {
        if (_activeImage != null) _activeImage.enabled = false;
        backgroundImage.enabled = true;
        highPriorityImage.enabled = false;
        mediumPriorityImage.enabled = false;
        lowPriorityImage.enabled = false;
        switch (priority) {
            case PriorityValue.Cant:
                backgroundImage.enabled = false;
                break;
            case PriorityValue.High:
                _activeImage = highPriorityImage;
                break;
            case PriorityValue.Low:
                _activeImage = lowPriorityImage;
                break;
            case PriorityValue.Medium:
                _activeImage = mediumPriorityImage;
                break;
            case PriorityValue.Wont:
                _activeImage = null;
                break;
                    default: break;
        }

        if (_activeImage != null) _activeImage.enabled = true;
    }

    public Button Button {
        get => button;
        set => button = value;
    }
}
