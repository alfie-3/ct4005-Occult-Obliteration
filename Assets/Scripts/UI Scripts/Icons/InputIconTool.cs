//Script that controls the inputs of the players

using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class InputIconTool {

    public static TmpSpriteAssets SpriteAssetList;

    public static string RetrieveSprite(InputBinding binding, string controlSchemeName) {

        DeviceType deviceType = RetrieveDeviceType(controlSchemeName);

        LoadSpriteListIfNotAlreadyLoaded();

        TMP_SpriteAsset spriteAsset = SpriteAssetList.SpriteAssets[(int)RetrieveDeviceType(controlSchemeName)];

        string buttonName = binding.ToString();
        string editedName = RenameInput(buttonName, deviceType);

        return ($"<sprite=\"{spriteAsset.name}\" name=\"{editedName}\">");
    }

    public static void LoadSpriteListIfNotAlreadyLoaded() {
        if (SpriteAssetList == null) {
            SpriteAssetList = Resources.Load("Sprite Assets/TMProSpriteAssets") as TmpSpriteAssets;
        }
    }

    private static string RenameInput(string inputButtonName, DeviceType deviceType) {
        string editedString = inputButtonName;

        string actionName = "";

        for (int i = 0; i < editedString.Length; i++) {
            if (editedString[i] != char.Parse(":")) {
                actionName += editedString[i];
            }
            else {
                actionName += editedString[i];
                break;
            }
        }

        editedString = editedString.Replace(actionName, string.Empty);

        switch (deviceType) {
            case DeviceType.Gamepad:
                editedString = editedString.Replace("<Gamepad>/", "Gamepad_");
                editedString = editedString.Replace("[Gamepad]", string.Empty);
                break;

            case DeviceType.Keyboard:
                editedString = editedString.Replace("<Keyboard>/", "Keyboard_");
                editedString = editedString.Replace("[KeyboardMouse]", string.Empty);
                editedString = editedString.Replace("[Keyboard&Mouse;Touch;Joystick;XR;KeyboardMouse]", string.Empty);
                break;
        }

            return editedString;
    }

    public static DeviceType RetrieveDeviceType(string controlSchemeName) {
        switch (controlSchemeName) {

            case ("KeyboardMouse"):
                return DeviceType.Keyboard;

            case ("Gamepad"):
                return DeviceType.Gamepad;
        }

        return DeviceType.Keyboard;
    }

    public enum DeviceType {
        Keyboard = 0,
        Gamepad = 1
    }
}
