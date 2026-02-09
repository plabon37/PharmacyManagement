using System;

namespace PharmacyManagement.Models
{
    public class Message
    {
        public int MessageId { get; set; }

        public int UserId { get; set; }

        public string MessageText { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsRead { get; set; } = false;
    }
}
