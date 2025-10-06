using System;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject userPanel;
    [SerializeField] private GameObject loadingPanel;

    [Header("Botones y Textos - Login")]
    [SerializeField] private Button loginButton;
    [SerializeField] private TMP_Text loginStatusText;

    [Header("Botones y Textos - Usuario")]
    [SerializeField] private TMP_Text playerIDTxt;
    [SerializeField] private TMP_Text playerNameTxt;
    [SerializeField] private TMP_InputField updateNameIF;
    [SerializeField] private Button updateNameBtn;
    [SerializeField] private Button continueToMenuBtn;
    [SerializeField] private Button signOutBtn;

    [Header("Configuración")]
    [SerializeField] private string menuSceneName = "MenuScene";

    private void Start()
    {
        ShowLoginPanel();
        UpdateLoginStatus("Presiona Login para comenzar");
    }

    private void OnEnable()
    {
        loginButton.onClick.AddListener(LoginButton);
        updateNameBtn.onClick.AddListener(UpdateName);
        continueToMenuBtn.onClick.AddListener(ContinueToMenu);
        signOutBtn.onClick.AddListener(SignOut);

        UnityPlayerAuth.Instance.OnSignedIn += OnSignedIn;
        UnityPlayerAuth.Instance.OnUpdateName += OnUpdateName;
        UnityPlayerAuth.Instance.OnAuthError += OnAuthError;
        UnityPlayerAuth.Instance.OnSignedOut += OnSignedOut;
    }

    private void OnDisable()
    {
        loginButton.onClick.RemoveListener(LoginButton);
        updateNameBtn.onClick.RemoveListener(UpdateName);
        continueToMenuBtn.onClick.RemoveListener(ContinueToMenu);
        signOutBtn.onClick.RemoveListener(SignOut);

        if (UnityPlayerAuth.Instance != null)
        {
            UnityPlayerAuth.Instance.OnSignedIn -= OnSignedIn;
            UnityPlayerAuth.Instance.OnUpdateName -= OnUpdateName;
            UnityPlayerAuth.Instance.OnAuthError -= OnAuthError;
            UnityPlayerAuth.Instance.OnSignedOut -= OnSignedOut;
        }
    }

    private void ShowLoginPanel()
    {
        loginPanel.SetActive(true);
        userPanel.SetActive(false);
        loadingPanel.SetActive(false);
    }

    private void ShowUserPanel()
    {
        loginPanel.SetActive(false);
        userPanel.SetActive(true);
        loadingPanel.SetActive(false);
    }

    private void ShowLoadingPanel(string message = "Cargando...")
    {
        loginPanel.SetActive(false);
        userPanel.SetActive(false);
        loadingPanel.SetActive(true);
        UpdateLoginStatus(message);
    }

    private void UpdateLoginStatus(string message)
    {
        if (loginStatusText != null)
            loginStatusText.text = message;
    }

    private async void LoginButton()
    {
        loginButton.interactable = false;
        ShowLoadingPanel("Iniciando sesión...");

        await UnityPlayerAuth.Instance.InitSignIn();

        loginButton.interactable = true;
    }

    private void OnSignedIn(PlayerInfo playerInfo, string playerName)
    {
        ShowUserPanel();
        playerIDTxt.text = "ID: " + playerInfo.Id;
        playerNameTxt.text = playerName;
        updateNameIF.text = "";
        UpdateLoginStatus("¡Login exitoso!");
    }

    private void OnUpdateName(string newName)
    {
        playerNameTxt.text = newName;
        updateNameIF.text = "";
    }

    private void OnAuthError(string error)
    {
        ShowLoginPanel();
        UpdateLoginStatus("Error: " + error);
    }

    private void OnSignedOut()
    {
        ShowLoginPanel();
        UpdateLoginStatus("Sesión cerrada. Presiona Login para comenzar.");
    }

    private async void UpdateName()
    {
        if (!string.IsNullOrEmpty(updateNameIF.text))
        {
            await UnityPlayerAuth.Instance.UpdatePlayerName(updateNameIF.text);
        }
    }

    private async void SignOut()
    {
        await UnityPlayerAuth.Instance.SignOut();
    }
    private void ContinueToMenu()
    {
        if (PlayerDataManager.Instance.IsDataLoaded)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(menuSceneName);
        }
        else
        {
            UpdateLoginStatus("Error: Datos no cargados");
        }
    }
}