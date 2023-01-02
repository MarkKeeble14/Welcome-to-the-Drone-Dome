using System.Collections;
using UnityEngine;
using TMPro;

public class ChangeMainMenuTextTrigger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mainMenuText;
    [SerializeField] private string[] textLines;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private bool oneTimeUse;
    [SerializeField] private Direction triggerDirection;
    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        // Check if colliding with Player
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, LayerMask.GetMask("Player"))) return;

        // Check if Player is moving right direction
        Vector2 moveDirection = InputManager._Controls.Player.Move.ReadValue<Vector2>();
        if (!DirectionHelper.GetDirectionOfMovement(moveDirection).Contains(triggerDirection)) return;

        // Check that trigger has not happened if is set to only happen once
        if (oneTimeUse && triggered) return;
        triggered = true;

        // Trigger
        mainMenuText.text = "";
        foreach (string line in textLines)
        {
            mainMenuText.text += line + "\n";
        }
        StartCoroutine(PromptLifetime());
    }

    private IEnumerator PromptLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        mainMenuText.text = "";
    }
}
