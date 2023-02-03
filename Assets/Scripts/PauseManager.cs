using System.Collections.Generic;
using UnityEngine;

public enum PauseCondition
{
    OPEN_SHOP,
    OPEN_UPGRADE,
    OPEN_FIRING_RANGE,
    OPEN_CREDIT_SHOP,
    ESCAPE,
    AWAKE
}

public class PauseManager : MonoBehaviour
{
    public static PauseManager _Instance { get; private set; }

    [SerializeField] private GameObject pauseCanvas;

    private List<PauseCondition> numPauseCommands = new List<PauseCondition>();

    public bool IsPaused { get; private set; }

    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(_Instance.gameObject);
        }
        _Instance = this;
        Resume(PauseCondition.AWAKE);
    }

    public void Pause(PauseCondition condition)
    {
        if (numPauseCommands.Contains(condition)) return;
        numPauseCommands.Add(condition);
        UpdatePauseState();
    }

    public void Resume(PauseCondition condition)
    {
        if (numPauseCommands.Contains(condition))
        {
            numPauseCommands.Remove(condition);
            UpdatePauseState();
        }
        else if (condition == PauseCondition.AWAKE)
        {
            UpdatePauseState();
        }
    }

    private void UpdatePauseState()
    {
        if (numPauseCommands.Contains(PauseCondition.ESCAPE))
        {
            Time.timeScale = 0;
            pauseCanvas.SetActive(true);

            IsPaused = true;
        }
        else if (numPauseCommands.Count > 0)
        {
            Time.timeScale = 0;
            pauseCanvas.SetActive(false);

            IsPaused = true;
        }
        else
        {
            Time.timeScale = 1;
            pauseCanvas.SetActive(false);

            IsPaused = false;
        }
    }
}
