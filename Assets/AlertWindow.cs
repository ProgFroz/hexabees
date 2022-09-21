using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlertWindow : MonoBehaviour {
    [SerializeField] private RawImage icon;
    [SerializeField] private TextMeshProUGUI text;
    public Alert _currentAlert;
    private List<Alert> _alerts = new List<Alert>();
    public bool _hidden;

    public float _timeLeft = 0;
    public float _secondsMax = 5f;
    public bool _transitioning = false;
    void Start() {
        _hidden = true;
        transform.localScale = Vector3.zero;
        CreateAlerts();
    }

    private void CreateAlerts() {
        _alerts.Add(new Alert(1, "No Food Stored!", AlertType.Food));
    }

    // Update is called once per frame
    void Update()
    {
        
        if (_currentAlert != null) {

            if (_timeLeft > 0) {
                _timeLeft -= Time.deltaTime;
            }
            if (_timeLeft <= 0 && !_hidden) {
                Hide();
            }
        }
    }

    public void UpdateAlert(AlertType type) {
        Alert alert = FindAlert(type);
        if (_currentAlert != null) {
            if (alert.Importance > _currentAlert.Importance) {
                HideAndShow(alert);
            }
        }
        else {
            Show(alert);
        }
    }

    private Alert FindAlert(AlertType type) {
        foreach (Alert alert in _alerts) {
            if (alert.Type == type) {
                return alert;
            }
        }

        return null;
    }

    private void Show(Alert alert) {
        _currentAlert = alert;
        text.text = alert.Message;
        if (!_transitioning) {
            _transitioning = true;
            LeanTween.scale(gameObject, Vector3.one, 0.1f).setEaseInOutCubic().setOnComplete(() => {
                _hidden = false;
                _transitioning = false;
                _timeLeft = _secondsMax;
            });
        }
    }

    private void Hide() {
        if (!_transitioning) {
            _transitioning = true;
            LeanTween.scale(gameObject, Vector3.zero, 0.1f).setEaseInOutCubic().setOnComplete(() => {
                _hidden = true;
                _transitioning = false;
                _currentAlert = null;
            });
        }
    }
    
    private void HideAndShow(Alert alert) {
        if (!_transitioning) {
            _transitioning = true;
            LeanTween.scale(gameObject, Vector3.zero, 0.1f).setEaseInOutCubic().setOnComplete(() => {
                _hidden = true;
                _transitioning = false;
                Show(alert);
            });
        }
    }
}

public class Alert {
    private int _importance;
    private string _message;
    private AlertType _type;

    public Alert(int importance, string message, AlertType type) {
        _importance = importance;
        _message = message;
        _type = type;
    }

    public int Importance {
        get => _importance;
        set => _importance = value;
    }

    public string Message {
        get => _message;
        set => _message = value;
    }

    public AlertType Type {
        get => _type;
        set => _type = value;
    }
}

public enum AlertType {
    Food,
    
}