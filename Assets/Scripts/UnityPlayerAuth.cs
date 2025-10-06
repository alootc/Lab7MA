using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
using System;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.CloudSave;

public class UnityPlayerAuth : MonoBehaviour
{
    public static UnityPlayerAuth Instance;

    public event Action<PlayerInfo, string> OnSignedIn;
    public event Action<string> OnUpdateName;
    public event Action OnSignedOut;
    public event Action<string> OnAuthError;

    private PlayerInfo playerInfo;
    private bool isInitialized = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private async void Start()
    {
        await InitializeUnityServices();
    }
    private async Task InitializeUnityServices()
    {
        try
        {
            await UnityServices.InitializeAsync();
            SetupAuthEvents();
            PlayerAccountService.Instance.SignedIn += SignInWithPlayerAccount;
            isInitialized = true;
            Debug.Log("Unity Services inicializado correctamente");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error inicializando Unity Services: " + ex.Message);
            OnAuthError?.Invoke("Error inicializando servicios: " + ex.Message);
        }
    }
    private void SetupAuthEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"Player ID: {AuthenticationService.Instance.PlayerId}");
            Debug.Log($"Token: {AuthenticationService.Instance.AccessToken}");
        };
        AuthenticationService.Instance.SignInFailed += (err) =>
        {
            Debug.LogError("Error en login: " + err);
            OnAuthError?.Invoke("Error en login: " + err.Message);
        };
        AuthenticationService.Instance.SignedOut += () =>
        {
            Debug.Log("Player cerró sesión");
            OnSignedOut?.Invoke();
        };
        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log("Sesión expirada");
            OnAuthError?.Invoke("Sesión expirada. Por favor, inicia sesión nuevamente.");
        };
    }
    public async Task InitSignIn()
    {
        if (!isInitialized)
        {
            OnAuthError?.Invoke("Servicios no inicializados");
            return;
        }
        try
        {
            await PlayerAccountService.Instance.StartSignInAsync();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error iniciando login: " + ex.Message);
            OnAuthError?.Invoke("Error iniciando sesión: " + ex.Message);
        }
    }
    private async void SignInWithPlayerAccount()
    {
        try
        {
            await SignInWithUnityAuth();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error en signIn: " + ex);
            OnAuthError?.Invoke("Error completando login: " + ex.Message);
        }
    }
    private async Task SignInWithUnityAuth()
    {
        try
        {
            string accessToken = PlayerAccountService.Instance.AccessToken;
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);

            playerInfo = AuthenticationService.Instance.PlayerInfo;
            var name = await AuthenticationService.Instance.GetPlayerNameAsync();

            Debug.Log("Login exitoso con Unity Player Accounts");

            await PlayerDataManager.Instance.LoadPlayerData();

            OnSignedIn?.Invoke(playerInfo, name);
        }
        catch (AuthenticationException ex)
        {
            Debug.LogError("Error de autenticación: " + ex);
            OnAuthError?.Invoke("Error de autenticación: " + ex.Message);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError("Error de request: " + ex);
            OnAuthError?.Invoke("Error de conexión: " + ex.Message);
        }
    }
    public async Task UpdatePlayerName(string newName)
    {
        try
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(newName);
            var name = await AuthenticationService.Instance.GetPlayerNameAsync();
            OnUpdateName?.Invoke(name);

            PlayerDataManager.Instance.UpdatePlayerName(name);

            Debug.Log("Nombre actualizado: " + name);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error actualizando nombre: " + ex.Message);
            OnAuthError?.Invoke("Error actualizando nombre: " + ex.Message);
        }
    }
    public async Task<string> GetPlayerName()
    {
        try
        {
            return await AuthenticationService.Instance.GetPlayerNameAsync();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error obteniendo nombre: " + ex.Message);
            return "Jugador";
        }
    }
    public async Task SignOut()
    {
        try
        {
            AuthenticationService.Instance.SignOut();
            Debug.Log("Sesión cerrada exitosamente");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error cerrando sesión: " + ex.Message);
        }
    }
    public async Task DeleteAccountUnityAsync()
    {
        try
        {
            await AuthenticationService.Instance.DeleteAccountAsync();
            Debug.Log("Cuenta eliminada exitosamente");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error eliminando cuenta: " + ex.Message);
            OnAuthError?.Invoke("Error eliminando cuenta: " + ex.Message);
        }
    }
    public bool IsAuthenticated()
    {
        return AuthenticationService.Instance.IsSignedIn;
    }
    public string GetPlayerId()
    {
        return AuthenticationService.Instance.PlayerId;
    }
}