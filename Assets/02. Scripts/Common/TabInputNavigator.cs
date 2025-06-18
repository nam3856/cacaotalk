using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class TabInputNavigator : MonoBehaviour
{
    public List<Selectable> inputFields;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable current = EventSystem.current.currentSelectedGameObject?.GetComponent<Selectable>();
            if (current != null)
            {
                int index = inputFields.IndexOf(current);
                if (index != -1)
                {
                    int nextIndex = (index + 1) % inputFields.Count;
                    inputFields[nextIndex].Select();
                }
            }
        }
    }
}
