using System;

namespace NewAddLogin.Data
{
    public class Ad
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
    }
}
