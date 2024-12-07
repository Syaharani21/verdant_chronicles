using UnityEngine;
using UnityEngine.Events;

[System.Serializable]

public class ResponeEvent
{
    [HideInInspector] public string name;
    [SerializeField] private UnityEvent onPickedResponse;

    public UnityEvent OnPickedResponse => onPickedResponse;
}
