using UnityEngine;
using UnityEngine.Events;

[System.Serializable]

public class ResponeEvent
{
    [HideaInInspector] public string name;
    [SerializeField] private UnityEvent onPickedResponse;

    public UnityEvent OnPickedResponse => onPickedResponse;
}
