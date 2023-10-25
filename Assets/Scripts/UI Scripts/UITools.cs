using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITools : MonoBehaviour
{
    public void SetSelected(GameObject selectable) {
        if (selectable.GetComponent<Selectable>() != null)
            selectable.GetComponent<Selectable>().Select();
    }
}
