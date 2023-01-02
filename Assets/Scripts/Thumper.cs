using System.Collections;
using UnityEngine;

public class Thumper : MonoBehaviour
{
    [SerializeField] private StatModifier numThumps;
    [SerializeField] private StatModifier timeBetweenThumps;
    [SerializeField] private ThumpRing ring;

    public void Thump()
    {
        StartCoroutine(CallThumps());
    }

    private IEnumerator CallThumps()
    {
        for (int i = 0; i < numThumps.Value; i++)
        {
            ThumpRing spawned = Instantiate(ring, transform);

            StartCoroutine(spawned.ExecuteThump(i == numThumps.Value - 1));

            yield return new WaitForSeconds(timeBetweenThumps.Value);
        }
    }
}