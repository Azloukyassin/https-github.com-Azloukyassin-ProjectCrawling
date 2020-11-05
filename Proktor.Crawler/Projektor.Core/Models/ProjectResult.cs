using System;

namespace Projektor.Core.Models
{
    public class ProjectResult
    {
        public bool Success { get; }
        public Exception Exception { get; }
        public Project Project { get; }

        public ProjectResult(bool success, Exception exception, Project project)
        {
            Success = success;
            Exception = exception;
            Project = project;
        }
    }
}
