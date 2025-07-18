PedaPal

A modern desktop app built for educators who want to simplify their workflow, store teaching resources securely, and never forget another task again. It combines cloud tools, reminders, and authentication in one slick WPF package.

Features

**Credential Management**  
  Securely store and retrieve credentials using the CredentialManagement library.

**Google Drive Integration**  
  Upload and access files directly from your Google Drive account.

**Firebase Authentication**  
  Authenticate users via Firebase Admin SDK with support for secure JSON-based credentials.

**Email Notifications**  
  Automatically send emails (e.g., reminders or alerts) to keep users informed.

**Reminder Service**  
  Schedule tasks and get notified when it’s time to act.

**MVVM Structure**  
  Clean and maintainable MVVM architecture using `RelayCommand` and data binding.

-------------------------------------------------------------------------------------------------------------

## 🛠 Tech Stack

- **C# (.NET Framework - WPF)**
- **Entity Framework 6**
- **Firebase Admin SDK**
- **Google Drive API**
- **CredentialManagement**
- **XAML for UI design**

------------------------------------------------------------------------------------------------------------

Project Structure
PedagogyPal/
├── Classes/ # Core services and helpers (Firebase, GoogleDrive, etc.)
├── Views/ # WPF UI views (XAML files)
├── App.xaml # Application entry config
├── App.config # Application configuration settings
├── PedagogyPal.csproj # Project file
├── packages.config # NuGet package references
