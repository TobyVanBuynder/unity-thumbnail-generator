using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ExitUI : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;

    Button _exitButton;

    void Awake()
    {
        if (_uiDocument == null)
        {
            enabled = false;
        }
    }

    void Start()
    {
        InitializeVariablesFromRoot(_uiDocument.rootVisualElement);
        HookButtons();
    }

    private void InitializeVariablesFromRoot(VisualElement root)
    {
        _exitButton = root.Q<Button>("ExitButton");
    }

    private void HookButtons()
    {
        _exitButton.clicked += OnExitButtonClicked;
    }

    private void OnExitButtonClicked()
    {
        Application.Quit();
    }
}
