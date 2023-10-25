using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class SoulPickup : Interactable
{
    [SerializeField] int soulAmount;

    GameObject currentPrompt;

    private TextMeshProUGUI tmpText;

    public void Awake() {
        tmpText = GetComponentInChildren<TextMeshProUGUI>(true);
    }

    public override void Interact(GameObject source) {
        source.GetComponent<PlayerManager>().ModifySouls(soulAmount, PlayerManager.SoulModification.add);
    }

    public override void UnView() {
        Destroy(currentPrompt);
    }

    public override void View(PlayerInput viewerInput) {
        if (currentPrompt != null)
            return;

        currentPrompt = Instantiate(PromptUtil.CreateInteractionPrompt(gameObject, new(0, 50), viewerInput), UIManager.Current.transform);
    }
}
