using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightingManager : MonoBehaviour
{
    public float timeProgressFactor = 1f;
    public float privateTimeProgressFactor = 1f;
    public float nightFasterFactor = 4f;
    public float nightBegin = 18f;
    public float nightEnd = 6f;
    public bool isActive;
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;
    [SerializeField] private GameObject skyboxManager;
    

    [SerializeField, Range(0,24)] public float TimeOfDay;
    public TimeManager timeManager;

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);
        if (DirectionalLight != null && this.isActive)
        {
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);
            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0f));
        }
    }
    private void OnValidate()
    {
        if (DirectionalLight != null) return;
        
    }
    // Start is called before the first frame update
    void Start()
    {
        privateTimeProgressFactor = timeProgressFactor;
    }

    // Update is called once per frame
    void Update()
    {
        if (Preset == null) return;
        if (Application.isPlaying)
        {
            if (TimeOfDay >= nightBegin || TimeOfDay <= nightEnd)
            {
                privateTimeProgressFactor = timeProgressFactor * nightFasterFactor;
            } 
            else
            {
                privateTimeProgressFactor = timeProgressFactor;
            }
            TimeOfDay += (Time.deltaTime * privateTimeProgressFactor);
            timeManager.UpdateTime((int)TimeOfDay);
            if (TimeOfDay >= 24)
            {
                TimeOfDay -= 24;
            }
            UpdateLighting(TimeOfDay / 24f);
        }
    }
}
