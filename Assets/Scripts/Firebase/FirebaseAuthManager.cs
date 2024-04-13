using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Google;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;
using Firebase.Firestore;
using Firebase.Storage;

public class FirebaseAuthManager : MonoBehaviour
{
    [Header("Game Version")]
    [SerializeField] public GameObject versionCover;
    string gameVersion = "1.1";
    [Header("Game Authentication")]
    [SerializeField] private Text warningSign;
    [SerializeField] private GameObject warningPanel;
    [SerializeField] public GameObject cover;
    [SerializeField] private GameObject authInfoPanel;
    [SerializeField] public Text authInfo;
    // Firebase variable
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;
    public UserData userData;
    public String authType = "";

    // Login Variables
    [Space]
    [Header("Login")]
    public InputField emailLoginField;
    public InputField passwordLoginField;

    // Registration Variables
    [Space]
    [Header("Registration")]
    public InputField nameRegisterField;
    public InputField emailRegisterField;
    public InputField passwordRegisterField;
    public InputField confirmPasswordRegisterField;

    [Header("Google Auth")]
    [SerializeField] Button googleSignInButton;
    public string GoogleWebAPI = "1035487390731-26fqcefdheuu25gffil2avo2uol82se6.apps.googleusercontent.com";
    private GoogleSignInConfiguration configuration;
    FirebaseFirestore firestore;
    FirebaseStorage storage;
    public QuerySnapshot queryDocuments;
    public List<string> listAkunGoogle;

    private async void Awake()
    {
        // Check that all of the necessary dependencies for firebase are present on the system
        await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;

            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
                configuration = new GoogleSignInConfiguration{
                    WebClientId = GoogleWebAPI,
                    RequestIdToken = true
                };
            }
            else
            {
                Debug.LogError("Could not resolve all firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void Start(){
        firestore = Firebase.Firestore.FirebaseFirestore.DefaultInstance;
        storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        authType = "";
        authInfo.text = "";
        authInfoPanel.SetActive(false);
        versionCover.SetActive(false);
        Debug.Log("Email: " + PlayerPrefs.GetString("email").ToString());
        Debug.Log("Pass: " + PlayerPrefs.GetString("pass").ToString());
        warningPanel.SetActive(false);
        warningSign.text = "";
        if(PlayerPrefs.GetString("email")!=""){
            SceneManager.LoadScene("MainMenu");
        }
        emailLoginField.onValueChanged.AddListener(OnEmailLoginFieldValueChanged);
        passwordLoginField.onValueChanged.AddListener(OnPassLoginFieldValueChanged);
        nameRegisterField.onValueChanged.AddListener(OnNameRegFieldValueChanged);
        emailRegisterField.onValueChanged.AddListener(OnEmailRegFieldValueChanged);
        passwordRegisterField.onValueChanged.AddListener(OnPassRegFieldValueChanged);
        confirmPasswordRegisterField.onValueChanged.AddListener(OnRePassRegFieldValueChanged);
        googleSignInButton.onClick.RemoveAllListeners();
        googleSignInButton.onClick.AddListener(()=>{
            GoogleSignClick();
        });
        FetchAndCheckVersion();
    }

    private void OnEmailLoginFieldValueChanged(string newText)
    {
        // Validate the input and remove emoticons if present
        string filteredText = RemoveEmoticons(newText);

        // Set the text to the filtered text
        emailLoginField.text = filteredText;
    }

    private void OnPassLoginFieldValueChanged(string newText)
    {
        // Validate the input and remove emoticons if present
        string filteredText = RemoveEmoticons(newText);

        // Set the text to the filtered text
        passwordLoginField.text = filteredText;
    }

    private void OnNameRegFieldValueChanged(string newText)
    {
        // Validate the input and remove emoticons if present
        string filteredText = RemoveEmoticons(newText);

        // Set the text to the filtered text
        nameRegisterField.text = filteredText;
    }

    private void OnEmailRegFieldValueChanged(string newText)
    {
        // Validate the input and remove emoticons if present
        string filteredText = RemoveEmoticons(newText);

        // Set the text to the filtered text
        emailRegisterField.text = filteredText;
    }

    private void OnPassRegFieldValueChanged(string newText)
    {
        // Validate the input and remove emoticons if present
        string filteredText = RemoveEmoticons(newText);

        // Set the text to the filtered text
        passwordRegisterField.text = filteredText;
    }

    private void OnRePassRegFieldValueChanged(string newText)
    {
        // Validate the input and remove emoticons if present
        string filteredText = RemoveEmoticons(newText);

        // Set the text to the filtered text
        confirmPasswordRegisterField.text = filteredText;
    }

    private string RemoveEmoticons(string text)
    {
        string allowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_=+[]{}|;:'\",.<>?/";
        string filteredText = "";

        foreach (char c in text)
        {
            if (allowedCharacters.Contains(c.ToString()))
            {
                filteredText += c;
            }
        }

        return filteredText;
    }

    public void CloseWarningSign(){
        warningPanel.SetActive(false);
        warningSign.text = "";
    }

    void InitializeFirebase()
    {
        //Set the default instance object
        auth = FirebaseAuth.DefaultInstance;

        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }

            user = auth.CurrentUser;
        }
    }

    public void Login()
    {
        authType = "Login";
        StartCoroutine(LoginAsync(emailLoginField.text, passwordLoginField.text));
    }

    private IEnumerator LoginAsync(string email, string password)
    {
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogError(loginTask.Exception);

            FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;

            string failedMessage = "Gagal Masuk! Karena ";
            
            switch (authError)
            {
                case AuthError.InvalidEmail:
                    failedMessage += "Email Salah";
                    break;
                case AuthError.WrongPassword:
                    failedMessage += "Password Salah";
                    break;
                case AuthError.MissingEmail:
                    failedMessage += "Email Tidak Ditemukan";
                    break;
                case AuthError.MissingPassword:
                    failedMessage += "Password Tidak Ditemukan";
                    break;
                default:
                    failedMessage = "Gagal Masuk";
                    break;
            }

            Debug.Log(failedMessage);
            warningSign.text = failedMessage;
            warningPanel.SetActive(true);
        }
        else
        {
            user = loginTask.Result;

            PlayerPrefs.SetString("email", emailLoginField.text);

            Debug.LogFormat("{0} You Are Successfully Logged In", user.DisplayName);
            PlayerPrefs.SetString("AuthType", "Manual");
            if(authType=="Login"){
                cover.SetActive(true);
                yield return new WaitForSecondsRealtime(2);
                authInfo.text = "Berhasil Login!";
                authInfoPanel.SetActive(true);
                yield return new WaitUntil(()=>Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
                authInfoPanel.SetActive(false);
                cover.SetActive(false);
                yield return new WaitForSecondsRealtime(0.1f);
                SceneManager.LoadScene("MainMenu");
            } else if (authType=="Register") {
                userData = GameObject.FindGameObjectWithTag("UserSystem").GetComponent<UserData>();
                userData.CreateUserData();
                yield return new WaitForSecondsRealtime(4);
                authInfo.text = "Berhasil Register!";
                authInfoPanel.SetActive(true);
                yield return new WaitUntil(()=>Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
            }
        }
    }

    public void Register()
    {
        authType = "Register";
        StartCoroutine(RegisterAsync(nameRegisterField.text, emailRegisterField.text, passwordRegisterField.text));
    }

    private IEnumerator RegisterAsync(string name, string email, string password)
    {
        if (name == "")
        {
            Debug.LogError("Name field is empty");
            warningSign.text = "Nama tidak boleh kosong!";
            warningPanel.SetActive(true);
        }
        else if (email == "")
        {
            Debug.LogError("Email field is empty");
            warningSign.text = "Email tidak boleh kosong!";
            warningPanel.SetActive(true);
        }
        else if (passwordRegisterField.text == "")
        {
            Debug.LogError("Password field is empty");
            warningSign.text = "Password tidak boleh kosong!";
            warningPanel.SetActive(true);
        }
        else if (confirmPasswordRegisterField.text == "")
        {
            Debug.LogError("Reconfirm Password field is empty");
            warningSign.text = "Password konfirmasi ulang tidak boleh kosong!";
            warningPanel.SetActive(true);
        }
        else if (passwordRegisterField.text != confirmPasswordRegisterField.text)
        {
            Debug.LogError("Password and Reconfirm Password does not match");
            warningSign.text = "Password dan Password Konfirmasi Tidak Sesuai!";
            warningPanel.SetActive(true);
        }
        else
        {
            cover.SetActive(true); 
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

            yield return new WaitUntil(() => registerTask.IsCompleted);

            if (registerTask.Exception != null)
            {
                Debug.LogError(registerTask.Exception);

                FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
                AuthError authError = (AuthError)firebaseException.ErrorCode;

                string failedMessage = "Registrasi Gagal! Karena ";
                switch (authError)
                {
                    case AuthError.InvalidEmail:
                        failedMessage += "Email Tidak Valid";
                        break;
                    case AuthError.WrongPassword:
                        failedMessage += "Password Salah";
                        break;
                    case AuthError.MissingEmail:
                        failedMessage += "Email Kosong";
                        break;
                    case AuthError.MissingPassword:
                        failedMessage += "Password Kosong";
                        break;
                    default:
                        failedMessage = "Email Sudah Digunakan!";
                        break;
                }

                Debug.Log(failedMessage);
                warningSign.text = failedMessage;
                warningPanel.SetActive(true);
            }
            else
            {
                // Get The User After Registration Success
                user = registerTask.Result;

                UserProfile userProfile = new UserProfile { DisplayName = name };

                var updateProfileTask = user.UpdateUserProfileAsync(userProfile);

                yield return new WaitUntil(() => updateProfileTask.IsCompleted);

                if (updateProfileTask.Exception != null)
                {
                    // Delete the user if user update failed
                    user.DeleteAsync();

                    Debug.LogError(updateProfileTask.Exception);

                    FirebaseException firebaseException = updateProfileTask.Exception.GetBaseException() as FirebaseException;
                    AuthError authError = (AuthError)firebaseException.ErrorCode;


                    string failedMessage = "Gagal Perbarui Profil! Karena ";
                    switch (authError)
                    {
                        case AuthError.InvalidEmail:
                            failedMessage += "Email Tidak Berlaku";
                            break;
                        case AuthError.WrongPassword:
                            failedMessage += "Password Salah";
                            break;
                        case AuthError.MissingEmail:
                            failedMessage += "Email Tidak Ditemukan";
                            break;
                        case AuthError.MissingPassword:
                            failedMessage += "Password Tidak Ditemukan";
                            break;
                        default:
                            failedMessage = "Gagal Perbarui Profil";
                            break;
                    }

                    Debug.Log(failedMessage);
                }
                else
                {
                    PlayerPrefs.SetString("email", email);
                    Debug.Log("Registration Sucessful Welcome " + user.DisplayName);
                    StartCoroutine(LoginAsync(email, password));
                }
            }
        }
    }

    private void GoogleSignClick(){
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleAuthenticatedFinishedAsync);
    }

    void OnGoogleAuthenticatedFinishedAsync(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            Debug.LogError("Fault");
        }
        else if (task.IsCanceled)
        {
            Debug.LogError("Login Cancel");
        }
        else
        {
            firestore.Collection("signedInGoogleAcc").GetSnapshotAsync().ContinueWithOnMainThread(getGoogleSignedIn =>
            {
                queryDocuments = getGoogleSignedIn.Result;

                listAkunGoogle = new List<string>();
                foreach (DocumentSnapshot document in queryDocuments.Documents)
                {
                    Dictionary<string, object> akun = document.ToDictionary();
                    string akunGoogle = new string((string)akun["token"]);
                    listAkunGoogle.Add(akunGoogle);
                }

                // Now, check if task.Result.UserId is present in listAkunGoogle
                if (listAkunGoogle.Contains(task.Result.UserId.ToString()))
                {
                    // UserId is present in the list, proceed with login
                    Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null);
                    auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(loginTask =>
                    {
                        if (loginTask.IsCanceled)
                        {
                            Debug.LogError("SignInWithCredentialAsync was canceled");
                            return;
                        }
                        if (loginTask.IsFaulted)
                        {
                            Debug.LogError("SignInWithCredentialAsync encountered an error: " + loginTask.Exception);
                            return;
                        }

                        user = auth.CurrentUser;
                        PlayerPrefs.SetString("email", user.Email);
                        PlayerPrefs.SetString("AuthType", "Google");
                        StartCoroutine(HandleLoginSuccess());
                    });
                }
                else
                {
                    // Execute only on the first sign-in (no token founded in signedInGoogleAcc)
                    Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null);
                    auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(signInTask =>
                    {
                        if (signInTask.IsCanceled)
                        {
                            Debug.LogError("SignInWithCredentialAsync was canceled");
                            return;
                        }
                        if (signInTask.IsFaulted)
                        {
                            Debug.LogError("SignInWithCredentialAsync encountered an error: " + signInTask.Exception);
                            return;
                        }

                        user = auth.CurrentUser;
                        PlayerPrefs.SetString("email", user.Email);
                        PlayerPrefs.SetString("AuthType", "Google");
                        Dictionary<string, object> googleAccountToken = new Dictionary<string, object>();
                        googleAccountToken.Add("token", task.Result.UserId.ToString());
                        googleAccountToken.Add("userUID", user.UserId.ToString());
                        firestore.Collection("signedInGoogleAcc").AddAsync(googleAccountToken).ContinueWithOnMainThread(createUserWrite => {
                            userData = GameObject.FindGameObjectWithTag("UserSystem").GetComponent<UserData>();
                            userData.CreateUserData();
                        });
                    });
                }
            });
        }
    }

    public IEnumerator HandleLoginSuccess()
    {
        cover.SetActive(true);
        yield return new WaitForSecondsRealtime(2);
        authInfo.text = "Berhasil Login!";
        authInfoPanel.SetActive(true);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
        authInfoPanel.SetActive(false);
        cover.SetActive(false);
        yield return new WaitForSecondsRealtime(0.1f);
        SceneManager.LoadScene("MainMenu");
    }

    void FetchAndCheckVersion()
    {
        firestore.Collection("version").Document("lle6usWZiQH859aLkjM9").GetSnapshotAsync().ContinueWithOnMainThread(taskVersion=>{
            Dictionary<string, object> versionInfo = taskVersion.Result.ToDictionary();
            string fetchedVersion = (string)versionInfo["current"];
            
            if (fetchedVersion != gameVersion)
            {
                versionCover.SetActive(true);
            }
        });
    }
}