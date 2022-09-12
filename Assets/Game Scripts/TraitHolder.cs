using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TraitHolder : MonoBehaviour {
    [SerializeField] private UIManager uiManager;
    [SerializeField] private RawImage image;
    [SerializeField] private Image background;
    [SerializeField] private Trait trait;

    private void Start() {
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    public void UpdateImage(Texture texture) {
        this.image.texture = texture;
    }
    public void UpdateBackground(Color color) {
        this.background.color = color;
    }

    public void UpdateTrait(Trait trait) {
        this.trait = trait;
        UpdateImage(uiManager.GetAccordingTexture(trait));
        UpdateBackground(uiManager.GetAccordingColorPositiveNegative(trait));
    }
}
