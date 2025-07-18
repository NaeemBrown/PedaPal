// UserModel.cs
using Google.Cloud.Firestore;
using System;

namespace PedagogyPal.Models
{
    [FirestoreData]
    public class UserModel
    {
        [FirestoreProperty]
        public string Email { get; set; }

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; }

        // Other user properties if any
    }
}
