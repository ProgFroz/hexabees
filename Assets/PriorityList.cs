using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityList : MonoBehaviour {
    
    [SerializeField] private BeeListElement elementPrefab;
    [SerializeField] private Transform content;
    private bool _hidden = true;
    private bool _transitioning = false;
    public void UpdateList(List<Bee> list) {
        foreach (Transform child in content) {
            Destroy(child.gameObject);
        }

        foreach (Bee bee in list) {
            GameObject listElement = Instantiate(elementPrefab.gameObject, content, false);
            BeeListElement beeListElement = listElement.GetComponent<BeeListElement>();
            beeListElement.UpdateElements(bee);
        }
    }

    private void Show() {
        if (!_transitioning) {
            _transitioning = true;
            LeanTween.scale(gameObject, Vector3.one, 0.1f).setEaseInOutCubic().setOnComplete(() => {
                _hidden = false;
                _transitioning = false;
            });
        }
    }

    private void Hide() {
        if (!_transitioning) {
            _transitioning = true;
            LeanTween.scale(gameObject, new Vector3(0.0001f, 0.0001f, 0.0001f), 0.1f).setEaseInOutCubic().setOnComplete(() => {
                _hidden = true;
                _transitioning = false;
            });
        }
    }

    public void Toggle() {
        if (_hidden) {
            Show();
        }
        else {
            Hide();
        }
    }
}
