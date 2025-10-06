using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;

public class AnonymAuth : MonoBehaviour
{
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        Debug.Log("Estado de Unity Services: " + UnityServices.State);
        SetupEvents();
        await SignInAnonymouslyAsync();
    }

    private void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Player ID: " + AuthenticationService.Instance.PlayerId);
            Debug.Log("Access Token: " + AuthenticationService.Instance.AccessToken);
        };

        AuthenticationService.Instance.SignInFailed += (err) =>
        {
            Debug.LogError("Error en login: " + err);
        };

        AuthenticationService.Instance.SignedOut += () =>
        {
            Debug.Log("Player cerró sesión");
        };

        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log("Sesión expirada");
        };
    }

    private async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Login anónimo exitoso");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogError("Error de autenticación: " + ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError("Error de request: " + ex);
        }
    }
}