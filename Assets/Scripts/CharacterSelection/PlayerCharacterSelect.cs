using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCharacterSelect : MonoBehaviour {

    //File references
    PlayerInput _playerInput;
    LocalMultiplayerManager _localMultiplayerManager;
    CharacterSelectionManager _characterSelectionManager;

    [Header("UI Elements")]
    [SerializeField] Image playerIcon;
    [SerializeField] Image confirmationIcon;
    [SerializeField] TextMeshProUGUI classNameText;
    [SerializeField] TextMeshProUGUI readyPrompt;
    [SerializeField] Sprite[] confirmationImages;

    [Header("Player Info")]
    //Relative player info
    public PlayerClassPlayableList characterClasses;
    [ReadOnly, SerializeField] int playerClassSelect = 0;

    //States
    bool canSelectCharacter = false;
    bool selectedCharacter = false;
    [ReadOnly, SerializeField] bool scrolling = false;

    private void OnEnable() {
        Subscribe();
    }

    private void OnDisable() {
        UnSubscribe();
    }


    //Start has alright stuff
    private void Start() {
        _playerInput = GetComponent<PlayerInput>();
        _localMultiplayerManager = LocalMultiplayerManager.Instance;
        _characterSelectionManager = CharacterSelectionManager.Instance;

        InputBinding binding = _playerInput.actions["Submit"].bindings[(int)InputIconTool.RetrieveDeviceType(_playerInput.currentControlScheme)];
        readyPrompt.text = $"{InputIconTool.RetrieveSprite(binding, _playerInput.currentControlScheme)} To Ready/Unready";

        _characterSelectionManager.playerSelectors.Add(this);
        _characterSelectionManager.PlayerReadyStatusChanged();

        transform.SetParent(GameObject.Find("PlayerSlotHolder").transform, false);

        confirmationIcon.enabled = false;
        PopulateSelectorCard();

        Invoke(nameof(EnableSelection), 0.2f);
    }

    public void NavigateCharacters(InputAction.CallbackContext context) {
        Vector2 navigationInput = context.ReadValue<Vector2>();

        if (!selectedCharacter && !scrolling && navigationInput.x != 0) {
            BrowseCharacters(navigationInput);
        }

        if (navigationInput.x == 0 && _playerInput.currentControlScheme == "KeyboardMouse") {
            scrolling = false;
        }
    }

    private async void BrowseCharacters(Vector2 input) {
        scrolling = true;

        if (input.x > 0.1f) {
            await ChangeCharacter("Right");
        }
        else if (input.x < -0.1f) {
            await ChangeCharacter("Left");
        }
    }

    private async Task ChangeCharacter(string direction) {
        if (direction == "Right") {

            if (playerClassSelect + 1 != characterClasses.playableClasses.Count) {
                playerClassSelect++;
            }
            else {
                playerClassSelect = 0;
            }
        }

        else if (direction == "Left") {

            if (playerClassSelect - 1 != -1) {
                playerClassSelect--;
            }
            else {
                playerClassSelect = characterClasses.playableClasses.Count - 1;
            }
        }

        PopulateSelectorCard();

        await Task.Delay((int)300);
        scrolling = false;
    }

    //Select character :)
    void Submit(InputAction.CallbackContext context) {
        if (!context.performed || !canSelectCharacter)
            return;

        if (confirmationIcon == null) return;

        if (!selectedCharacter) {
            bool assignClass = true;
            foreach (Player player in LocalMultiplayerManager.Instance.players) {
                if (player.PlayerClass == characterClasses.playableClasses[playerClassSelect]) {
                    assignClass = false;
                    _characterSelectionManager.PlayerReadyStatusChanged();
                }
            }
            if (!selectedCharacter && assignClass) {
                AssignClass(characterClasses.playableClasses[playerClassSelect]);
                confirmationIcon.enabled = true;
                selectedCharacter = true;
                _characterSelectionManager.PlayerReadyStatusChanged();
            }
        }
        else {
            Cancel(context);
            _characterSelectionManager.PlayerReadyStatusChanged();
        }
    }

    //Deselect character
    void Cancel(InputAction.CallbackContext context) {
        if (selectedCharacter) {
            selectedCharacter = false;
            confirmationIcon.enabled = false;
        }
        _characterSelectionManager.PlayerReadyStatusChanged();
        _localMultiplayerManager.players[_playerInput.playerIndex].PlayerClass = null;
    }

    //Start game
    void StartGame(InputAction.CallbackContext context) {
        bool startGame = true;
        foreach (Player player in LocalMultiplayerManager.Instance.players) {
            if (player.PlayerClass == null) {
                startGame = false;
            }
        }
        if (startGame) {
            SceneManager.LoadScene(2);
        }
    }

    //Implement on select
    public void AssignClass(PlayerClass selectedClass) {
        if (LocalMultiplayerManager.Instance != null)
            LocalMultiplayerManager.Instance.players[_playerInput.playerIndex].PlayerClass = selectedClass;
    }

    public void PopulateSelectorCard() {
        if (CharacterSelectionManager.Instance.ClassAlreadySelected(characterClasses.playableClasses[playerClassSelect], _playerInput.playerIndex)) {
            confirmationIcon.sprite = confirmationImages[1];
            confirmationIcon.enabled = true;
        }
        else {
            confirmationIcon.sprite = confirmationImages[0];

            if (!selectedCharacter)
            confirmationIcon.enabled = false;
        }

        playerIcon.sprite = characterClasses.playableClasses[playerClassSelect].ClassInfo.selectImage;
        classNameText.text = characterClasses.playableClasses[playerClassSelect].ClassInfo.name;
    }

    public bool HasSelected() => selectedCharacter;
    private void EnableSelection() => canSelectCharacter = true;

    private void OnDestroy() {
        UnSubscribe();
    }

    private void Subscribe() {
        PlayerInput input = GetComponent<PlayerInput>();

        input.actions["Submit"].performed += Submit;
        input.actions["Cancel"].performed += Cancel;
        input.actions["StartButton"].performed += StartGame;

        input.actions["Navigate"].performed += NavigateCharacters;
        input.actions["Navigate"].canceled += NavigateCharacters;
    }
    private void UnSubscribe() {
        PlayerInput input = GetComponent<PlayerInput>();

        input.actions["Click"].performed -= Submit;
        input.actions["Cancel"].performed -= Cancel;
        input.actions["StartButton"].performed -= StartGame;

        input.actions["Navigate"].performed -= NavigateCharacters;
    }
}
