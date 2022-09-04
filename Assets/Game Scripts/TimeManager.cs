using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public float pausedTimeScale;
    public bool gameIsPaused;
    public UIManager uiManager;
    public int day = 0;
    public int hour = 0;
    public int year = 0;
    public int season = 0;
    private void Start()
    {
        this.gameIsPaused = false;
        this.SetNormalSpeed();
    }

    private void Update()
    {
        //uiManager.UpdateDateTimeUI(day, hour);

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            TogglePause();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetNormalSpeed();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetHigherSpeed();
        }
    }

    public void PauseGame()
    {
        if (gameIsPaused) return;
        pausedTimeScale = Time.timeScale;
        Time.timeScale = 0;
        uiManager.SetPauseButtonActive();
        this.gameIsPaused = true;
    }

    public void ResumeGame()
    {
        if (!gameIsPaused) return;
        Time.timeScale = pausedTimeScale;
        uiManager.SetPauseButtonInactive();
        this.gameIsPaused = false;
    }

    public void TogglePause()
    {
        if (!gameIsPaused) PauseGame();
        else ResumeGame();
    }

    public void SetNormalSpeed()
    {
        if (this.gameIsPaused) return;
        Time.timeScale = 1f;
        uiManager.SetNormalSpeedButtonActive();
    }

    public void SetHigherSpeed()
    {
        if (this.gameIsPaused) return;
        Time.timeScale = 2f;
        uiManager.SetDoubleSpeedButtonActive();
    }

    public void UpdateTime(int hour) {
        this.hour = hour;

        if (hour >= 24) {
            this.hour = 0;
            this.day += 1;
        }

        if (day >= 31) {
            this.day = 0;
            this.season += 1;
        }

        if (season >= 4) {
            this.season = 0;
            this.year += 1;
        }
        
        uiManager.UpdateDateTimeUI(hour, day, season, year);
    }
}

public enum Season {
    Spring,
    Summer,
    Fall,
    Winter
}
