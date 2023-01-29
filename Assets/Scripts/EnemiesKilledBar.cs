using TMPro;
using UnityEngine;

public class EnemiesKilledBar : ProgressBar
{

    private int waveCounter = 1;
    public int WavesCompleted => waveCounter;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private string prefix;

    public void IncrementCounter()
    {
        waveCounter++;
        SetText(waveCounter.ToString());
    }

    public void SetText(string text)
    {
        this.text.text = prefix + text;
    }
}
