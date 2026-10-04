using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;




// Drives the login/signup screen. Wire the fields in the Inspector.
public class MainMenuUI : MonoBehaviour
{
    [Header("Inputs")]
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;




    [Header("Buttons")]
    public Button loginButton;
    public Button signUpButton;




    [Header("Show/hide password (optional)")]
    public Button showPasswordButton;




    [Header("Feedback")]
    public TMP_Text statusText;




    [Header("Scene to load after signing in")]
    public string menuSceneName = "Main_Menu_normalno";




    bool passwordVisible;




    void Start()
    {
        loginButton.onClick.AddListener(() => Submit(isSignUp: false));
        signUpButton.onClick.AddListener(() => Submit(isSignUp: true));




        if (showPasswordButton != null)
        {
            showPasswordButton.onClick.AddListener(TogglePassword);
            ApplyPasswordVisibility();
        }




        // A saved token means they're already signed in, so skip the form entirely.
        // The token can still be expired or revoked, which /stats tells us.
        if (EcoHouseApi.Instance != null && EcoHouseApi.Instance.IsLoggedIn)
        {
            SetStatus($"Welcome back, {EcoHouseApi.Instance.Username}.");
            SetInteractable(false);
            StartCoroutine(ResumeSession());
        }
    }




    System.Collections.IEnumerator ResumeSession()
    {
        yield return EcoHouseApi.Instance.GetStats((ok, stats, error) =>
        {
            if (ok)
            {
                SceneManager.LoadScene(menuSceneName);
                return;
            }




            // Expired or unreachable: fall back to the form rather than hanging.
            SetInteractable(true);
            SetStatus(error);
        });
    }




    void TogglePassword()
    {
        passwordVisible = !passwordVisible;
        ApplyPasswordVisibility();
    }




    void ApplyPasswordVisibility()
    {
        passwordField.contentType = passwordVisible
            ? TMP_InputField.ContentType.Standard
            : TMP_InputField.ContentType.Password;




        // Without this the field keeps drawing the old masking until it's re-focused.
        passwordField.ForceLabelUpdate();
    }




    void Submit(bool isSignUp)
    {
        var username = usernameField.text.Trim();
        var password = passwordField.text;




        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            SetStatus("Enter a username and password.");
            return;
        }




        SetInteractable(false);
        SetStatus(isSignUp ? "Creating your account..." : "Signing in...");




        var api = EcoHouseApi.Instance;
        var routine = isSignUp
            ? api.SignUp(username, password, OnResult)
            : api.LogIn(username, password, OnResult);




        StartCoroutine(routine);
    }




    void OnResult(bool ok, string error)
    {
        if (!ok)
        {
            SetInteractable(true);
            SetStatus(error);
            return;
        }




        SetStatus("Welcome!");
        SceneManager.LoadScene(menuSceneName);
    }




    void SetInteractable(bool value)
    {
        loginButton.interactable = value;
        signUpButton.interactable = value;
        usernameField.interactable = value;
        passwordField.interactable = value;
    }




    void SetStatus(string message)
    {
        if (statusText != null) statusText.text = message ?? "";
    }
}