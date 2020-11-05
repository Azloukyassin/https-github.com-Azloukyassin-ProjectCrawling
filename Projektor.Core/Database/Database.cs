using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Projektor.Core.Enums;
using Projektor.Core.Models;

namespace Projektor.Core.Database
{
    public class Database
    {
        //Settings
        private string serverAdresse = "mongodb://localhost:27017";
        private IMongoDatabase _database;

        public Database()
        {
            _database = new MongoClient(serverAdresse).GetDatabase(nameof(Project));
        }

        public async Task<Task> StoreProjects(CrawlerName crawlerName, Project project)
        {
            var collection = _database.GetCollection<Project>(crawlerName.ToString());
            var filter = Builders<Project>.Filter.Eq(x => x.ReferenceNumber, project.ReferenceNumber);

            var storedProject = await collection.Find(filter).SingleOrDefaultAsync();
            if (storedProject != null)
            {
                if (HasChanges(storedProject, project))
                {
                    project.Id = storedProject.Id;
                    project.IsNew = true;
                    await collection.ReplaceOneAsync(filter, project);
                }
            }
            else
            {
                project.IsNew = true;
                await collection.InsertOneAsync(project);
            }
            return Task.CompletedTask;
        }

        private bool HasChanges(Project storedProject, Project project)
        {
            foreach (var property in typeof(Project).GetProperties())
            {
                if (property.Name.Equals(nameof(Project.Id)))
                    continue;
                var storedProjectProperty = property.GetValue(storedProject);
                var projectProperty = property.GetValue(project);

                if (projectProperty is List<string> propertyList)
                {
                    var storedList = storedProjectProperty as List<string>;
                    if (storedList == null && propertyList.Any())
                        return true;
                    if (propertyList.Any() && storedList.Any())
                    {
                        var newListItems = propertyList.Except(storedList).ToList();
                        if (newListItems.Any())
                            return true;
                    }
                }
                else if (projectProperty is Contact contact)
                {
                    if (!contact.Equals(storedProject.Contact))
                        return true;
                }
                else if (!storedProjectProperty.Equals(projectProperty))
                {
                    return true;
                }
            }
            return false;
        }
    }

}
