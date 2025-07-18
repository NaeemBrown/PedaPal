// TaskModel.cs
using Google.Cloud.Firestore;
using System;

namespace PedagogyPal.Models
{
    [FirestoreData] // Indicates that this class is Firestore serializable
    public class TaskModel
    {
        [FirestoreProperty] // Maps the Firestore field "Id" to this property
        public string Id { get; set; }

        [FirestoreProperty]
        public string Title { get; set; }

        [FirestoreProperty]
        public string Description { get; set; }

        [FirestoreProperty]
        public DateTime DueDate { get; set; }

        [FirestoreProperty]
        public string Status { get; set; }

        [FirestoreProperty]
        public string DocumentLink { get; set; }

        [FirestoreProperty]
        public string UserId { get; set; }

        // Public parameterless constructor required by Firestore
        public TaskModel() { }

        // Optional: Parameterized constructor for convenience
        public TaskModel(string id, string title, string description, DateTime dueDate, string status, string documentLink, string userId)
        {
            Id = id;
            Title = title;
            Description = description;
            DueDate = dueDate;
            Status = status;
            DocumentLink = documentLink;
            UserId = userId;
        }
    }
}
