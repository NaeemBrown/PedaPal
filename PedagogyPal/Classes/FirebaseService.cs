// FirebaseService.cs
using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PedagogyPal.Models;
using PedagogyPal.Helpers;

namespace PedagogyPal.Services
{
    public class FirebaseService
    {
        private const string ApiKey = "xxxyyyzzz";

        private readonly FirestoreDb _firestoreDb;

        // Define CredentialTarget for Credential Management
        private const string CredentialTarget = "PedagogyPal.Firebase";

        // Property to store the current user's ID
        public string CurrentUserId { get; private set; }

        // Events to notify when user logs in or out
        public event EventHandler UserLoggedIn;
        public event EventHandler UserLoggedOut;

        public FirebaseService()
        {
            // Path to the Firebase credentials JSON file
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "pedapal-66092-firebase-adminsdk-kfch3-b7003c1394.json");

            // Ensure the credentials file exists
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Firebase credentials file not found at {path}");
            }

            // Set the environment variable for Google credentials
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            // Initialize FirestoreDb with the project ID
            _firestoreDb = FirestoreDb.Create("pedapal-xxyy"); // 
            Console.WriteLine("Connected to Firestore Database");
        }

        #region User Authentication

        /// <summary>
        /// Registers a new user using Firebase Authentication API.
        /// </summary>
        public async Task<(bool Success, string Message, string IdToken)> RegisterUserAsync(string email, string password, bool rememberMe)
        {
            using (var client = new HttpClient())
            {
                var requestUri = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={ApiKey}";

                var payload = new
                {
                    email = email,
                    password = password,
                    returnSecureToken = true
                };

                var jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(requestUri, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("User registered successfully.");

                    // Parse the response to extract tokens
                    var authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseContent);

                    if (rememberMe)
                    {
                        CredentialHelper.SaveRefreshToken(email, authResponse.refreshToken);
                    }
                    else
                    {
                        CredentialHelper.RemoveCredentials();
                    }

                    // Add user document to Firestore
                    await AddUserDocumentAsync(authResponse.localId, email);

                    // Set the current user ID
                    CurrentUserId = authResponse.localId;

                    // Raise UserLoggedIn event
                    UserLoggedIn?.Invoke(this, EventArgs.Empty);

                    return (true, "Registration successful.", authResponse.idToken);
                }
                else
                {
                    Console.WriteLine("Registration failed.");
                    Console.WriteLine("Error details: " + responseContent); // Log error details for troubleshooting

                    // Extract error message from response
                    var errorResponse = JsonConvert.DeserializeObject<AuthErrorResponse>(responseContent);
                    string errorMessage = errorResponse?.error?.message ?? "Unknown error.";

                    return (false, $"Registration failed: {errorMessage}", null);
                }
            }
        }

        /// <summary>
        /// Logs in an existing user using Firebase Authentication API.
        /// </summary>
        public async Task<(bool Success, string Message, string IdToken)> LoginUserAsync(string email, string password, bool rememberMe)
        {
            using (var client = new HttpClient())
            {
                var requestUri = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={ApiKey}";

                var payload = new
                {
                    email = email,
                    password = password,
                    returnSecureToken = true
                };

                var jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(requestUri, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("User logged in successfully.");

                    // Parse the response to extract tokens
                    var authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseContent);

                    if (rememberMe)
                    {
                        CredentialHelper.SaveRefreshToken(email, authResponse.refreshToken);
                    }
                    else
                    {
                        CredentialHelper.RemoveCredentials();
                    }

                    // Set the current user ID
                    CurrentUserId = authResponse.localId;

                    // Raise UserLoggedIn event
                    UserLoggedIn?.Invoke(this, EventArgs.Empty);

                    return (true, "Login successful.", authResponse.idToken);
                }
                else
                {
                    Console.WriteLine("Login failed.");
                    Console.WriteLine("Error details: " + responseContent); // Log error details for troubleshooting

                    // Extract error message from response
                    var errorResponse = JsonConvert.DeserializeObject<AuthErrorResponse>(responseContent);
                    string errorMessage = errorResponse?.error?.message ?? "Unknown error.";

                    return (false, $"Login failed: {errorMessage}", null);
                }
            }
        }

        /// <summary>
        /// Attempts to auto-login the user using stored refresh tokens.
        /// </summary>
        public async Task<(bool Success, string Message, string IdToken)> AutoLoginAsync()
        {
            var credentials = CredentialHelper.GetRefreshToken();
            if (!string.IsNullOrEmpty(credentials.RefreshToken))
            {
                try
                {
                    // Refresh the idToken using the refreshToken
                    var refreshedTokens = await RefreshIdTokenAsync(credentials.RefreshToken);
                    if (refreshedTokens != null)
                    {
                        // Optionally, update the refreshToken if it has changed
                        CredentialHelper.SaveRefreshToken(credentials.Username, refreshedTokens.refresh_token);

                        // Set the current user ID
                        CurrentUserId = refreshedTokens.user_id;

                        // Raise UserLoggedIn event
                        UserLoggedIn?.Invoke(this, EventArgs.Empty);

                        return (true, "Auto-login successful.", refreshedTokens.id_token);
                    }
                }
                catch (Exception ex)
                {
                    // Token might be expired or invalid
                    Console.WriteLine($"AutoLogin Error: {ex.Message}");
                    CredentialHelper.RemoveCredentials();
                }
            }
            return (false, "No credentials stored or auto-login failed.", null);
        }

        /// <summary>
        /// Logs out the user by removing stored credentials.
        /// </summary>
        public void Logout()
        {
            CredentialHelper.RemoveCredentials();
            CurrentUserId = null;

            // Raise UserLoggedOut event
            UserLoggedOut?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Refreshes the IdToken using the provided refreshToken.
        /// </summary>
        private async Task<RefreshTokenResponse> RefreshIdTokenAsync(string refreshToken)
        {
            using (var client = new HttpClient())
            {
                var requestUri = $"https://securetoken.googleapis.com/v1/token?key={ApiKey}";

                var payload = new
                {
                    grant_type = "refresh_token",
                    refresh_token = refreshToken
                };

                var jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(requestUri, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Token refreshed successfully.");
                    var refreshResponse = JsonConvert.DeserializeObject<RefreshTokenResponse>(responseContent);
                    return refreshResponse;
                }
                else
                {
                    Console.WriteLine("Token refresh failed.");
                    Console.WriteLine("Error details: " + responseContent);
                    return null;
                }
            }
        }

        #endregion

        #region Firestore User Data Management

        /// <summary>
        /// Adds a user document to the "users" collection in Firestore.
        /// </summary>
        public async Task AddUserDocumentAsync(string userId, string email)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));

            var userDoc = new UserModel
            {
                Email = email,
                CreatedAt = DateTime.UtcNow
            };
            DocumentReference docRef = _firestoreDb.Collection("users").Document(userId);
            await docRef.SetAsync(userDoc);
            Console.WriteLine("User document added to Firestore");
        }

        /// <summary>
        /// Retrieves the email address of a user by their user ID.
        /// </summary>
        public async Task<string> GetUserEmailAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            DocumentReference docRef = _firestoreDb.Collection("users").Document(userId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                var user = snapshot.ConvertTo<UserModel>();
                return user.Email;
            }
            return null;
        }

        #endregion

        #region Task Management

        /// <summary>
        /// Adds a new task to the "tasks" collection and assigns the generated Document ID.
        /// </summary>
        public async Task<TaskModel> AddTaskAsync(TaskModel task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task), "TaskModel cannot be null");
            }

            CollectionReference tasksRef = _firestoreDb.Collection("tasks");
            DocumentReference docRef = await tasksRef.AddAsync(task);
            task.Id = docRef.Id; // Assign the generated Document ID
            Console.WriteLine("Task added to Firestore");
            return task;
        }

        public FirestoreDb GetFirestoreDb()
        {
            return _firestoreDb;
        }

        /// <summary>
        /// Retrieves all tasks from the "tasks" collection.
        /// </summary>
        public async Task<List<TaskModel>> GetTasksAsync()
        {
            var tasks = new List<TaskModel>();
            Query tasksQuery = _firestoreDb.Collection("tasks").WhereEqualTo("UserId", CurrentUserId);
            QuerySnapshot snapshot = await tasksQuery.GetSnapshotAsync();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                TaskModel task = document.ConvertTo<TaskModel>();
                task.Id = document.Id; // Assign Firestore Document ID
                tasks.Add(task);
            }

            return tasks;
        }

        /// <summary>
        /// Deletes a specific task from the "tasks" collection using its Id.
        /// </summary>
        public async Task<bool> DeleteTaskAsync(TaskModel task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (string.IsNullOrEmpty(task.Id))
                throw new ArgumentException("Task ID cannot be null or empty.", nameof(task.Id));

            DocumentReference docRef = _firestoreDb.Collection("tasks").Document(task.Id);
            await docRef.DeleteAsync();
            Console.WriteLine($"Task '{task.Title}' deleted from Firestore.");
            return true;
        }

        /// <summary>
        /// Updates an existing task in the "tasks" collection using its Id.
        /// </summary>
        public async Task<bool> UpdateTaskAsync(TaskModel task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (string.IsNullOrEmpty(task.Id))
                throw new ArgumentException("Task ID cannot be null or empty.", nameof(task.Id));

            DocumentReference docRef = _firestoreDb.Collection("tasks").Document(task.Id);
            await docRef.SetAsync(task, SetOptions.Overwrite);
            Console.WriteLine($"Task '{task.Title}' updated in Firestore.");
            return true;
        }

        #endregion
    }

    // Define classes to deserialize Firebase responses

    /// <summary>
    /// Represents the successful authentication response from Firebase.
    /// </summary>
    public class AuthResponse
    {
        public string idToken { get; set; }
        public string refreshToken { get; set; }
        public string expiresIn { get; set; }
        public string localId { get; set; }
        public bool registered { get; set; }
    }

    /// <summary>
    /// Represents the response received when refreshing tokens.
    /// </summary>
    public class RefreshTokenResponse
    {
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public string id_token { get; set; }
        public string user_id { get; set; }
        public string project_id { get; set; }
    }

    /// <summary>
    /// Represents the error response from Firebase Authentication API.
    /// </summary>
    public class AuthErrorResponse
    {
        public AuthError error { get; set; }
    }

    public class AuthError
    {
        public string code { get; set; }
        public string message { get; set; }
        public List<AuthErrorDetail> errors { get; set; }
    }

    public class AuthErrorDetail
    {
        public string message { get; set; }
        public string domain { get; set; }
        public string reason { get; set; }
    }
}
