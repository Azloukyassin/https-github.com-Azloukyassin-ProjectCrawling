using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SalesEngine.Enums;

namespace Projektor.Core.Models
{ 
    public class Status
    {
        public Status(State state, IEnumerable<string> errors)
        {
            State = state;
            Errors = errors;
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public State State { get; }
        public IEnumerable<string> Errors { get; }
    }
}
