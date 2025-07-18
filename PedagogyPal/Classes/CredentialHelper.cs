// CredentialHelper.cs
using CredentialManagement;
using System;

namespace PedagogyPal.Helpers
{
    public static class CredentialHelper
    {
        private const string Target = "PedagogyPal.Firebase";

        /// <summary>
        /// Saves the refresh token securely in the Windows Credential Manager.
        /// </summary>
        /// <param name="username">The user's email address.</param>
        /// <param name="refreshToken">The Firebase refresh token.</param>
        public static void SaveRefreshToken(string username, string refreshToken)
        {
            using (var cred = new Credential())
            {
                cred.Target = Target;
                cred.Username = username;
                cred.Password = refreshToken;
                cred.Type = CredentialType.Generic;
                cred.PersistanceType = PersistanceType.LocalComputer;
                cred.Save();
            }
        }

        /// <summary>
        /// Retrieves the stored refresh token from the Windows Credential Manager.
        /// </summary>
        /// <returns>A tuple containing the username and refresh token.</returns>
        public static (string Username, string RefreshToken) GetRefreshToken()
        {
            using (var cred = new Credential())
            {
                cred.Target = Target;
                cred.Type = CredentialType.Generic;
                if (cred.Load())
                {
                    return (cred.Username, cred.Password);
                }
            }
            return (null, null);
        }

        /// <summary>
        /// Removes the stored credentials from the Windows Credential Manager.
        /// </summary>
        public static void RemoveCredentials()
        {
            using (var cred = new Credential())
            {
                cred.Target = Target;
                cred.Load();
                cred.Delete();
            }
        }
    }
}
