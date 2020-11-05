using System.Collections.Generic;

namespace Projektor.WebApp.Pages.Components.TdUl
{
    public class TdUlModel
    {
        public IEnumerable<string> Values { get; set; }

        public TdUlModel(IEnumerable<string> values)
        {
            Values = values;
        }
    }
}
