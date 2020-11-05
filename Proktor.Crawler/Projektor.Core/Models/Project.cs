using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Projektor.Core.Services.Crawlers.ComputerFutures;
using Projektor.Core.Services.Crawlers.Hays;

namespace Projektor.Core.Models
{
    public class Project
    {
        public Project(string title, string location, string referenceNumber, string startingDate, string duration, string position, IEnumerable<string> tasks, IEnumerable<string> qualifications, IEnumerable<string> advantages, IEnumerable<string> incomeInformation, IEnumerable<string> projectInformation, IEnumerable<string> employerInformation, IEnumerable<string> employmentInformation, Contact contact, Uri uri)
        {
            Title = title;
            Location = location;
            ReferenceNumber = referenceNumber;
            StartingDate = startingDate;
            Duration = duration;
            Position = position;
            Tasks = tasks;
            Qualifications = qualifications;
            Advantages = advantages;
            IncomeInformation = incomeInformation;
            ProjectInformation = projectInformation;
            EmployerInformation = employerInformation;
            EmploymentInformation = employmentInformation;
            Contact = contact;
            Uri = uri;  
        }
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }
        [BsonElement("Titel")]
        public string Title { get; set; }
        [BsonElement("Location")]
        public string Location { get; set; }
        [BsonElement("ReferenceNumber")]
        public string ReferenceNumber { get; set; }
        [BsonElement("StartingDate")]
        public string StartingDate { get; set; }
        [BsonElement("Duration")]
        public string Duration { get; set; }
        [BsonElement("Position")]
        public string Position { get; set; }
        [BsonElement("Tasks")]
        public IEnumerable<string> Tasks { get; set; }
        [BsonElement("Qualifications")]
        public IEnumerable<string> Qualifications { get; set; }
        [BsonElement("Advantages")]
        public IEnumerable<string> Advantages { get; set; }
        [BsonElement("IncomeInformation")]
        public IEnumerable<string> IncomeInformation { get; set; }
        [BsonElement("ProjectInformation")]
        public IEnumerable<string> ProjectInformation { get; set; }
        [BsonElement("EmployerInformation")]
        public IEnumerable<string> EmployerInformation { get; set; }
        [BsonElement("EmploymentInformation")]
        public IEnumerable<string> EmploymentInformation { get; set; }
        [BsonElement("Contact")]
        public Contact Contact { get; set; }
        [BsonElement("Uri")]
        public Uri Uri { get; set; }
        [BsonElement]
        public bool IsNew { get; set; }
        public bool HasAnsprechpartner => Contact.Name != HaysDataParser.NoInformation;
        public bool HasTelephoneNumber => Contact.TelephoneNumber != HaysDataParser.NoInformation;
        public bool HasEmailNumber => Contact.Email != HaysDataParser.NoInformation;

        public bool HasContact => HasAnsprechpartner || HasTelephoneNumber || HasEmailNumber;

    }
}
