using System;

namespace Projektor.Core.Models
{
    public class Contact : IEquatable<Contact>
    {
        public string Name { get; set; }
        public string TelephoneNumber { get; set; }
        public string Email { get; set; }

        public Contact(string name, string telephoneNumber, string email)
        {
            Name = name;
            TelephoneNumber = telephoneNumber;
            Email = email;
        }

        public bool Equals(Contact other)
        {
            if (ReferenceEquals(null, other)) return false;
            return Name == other.Name && TelephoneNumber == other.TelephoneNumber && Email == other.Email;
        }
    }
}
