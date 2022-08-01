using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HousingRegisterApi.V1.Infrastructure.Search
{
    public class ApplicationSearchSemiStructuredQuery
    {
        const string ExpertSymbols = "+|\"*()~";

        public string OriginalQuery { get; set; }

        public List<string> Terms { get; set; }

        public List<DateTime> Dates { get; set; }

        public List<string> ReferenceNumbers { get; set; }

        public List<string> NINOs { get; set; }

        public List<Guid> ApplicationIds { get; set; }

        public static ApplicationSearchSemiStructuredQuery Empty
        {
            get
            {
                return new ApplicationSearchSemiStructuredQuery
                {
                    OriginalQuery = "",
                    ApplicationIds = new List<Guid>(),
                    Dates = new List<DateTime>(),
                    NINOs = new List<string>(),
                    ReferenceNumbers = new List<string>(),
                    Terms = new List<string>()
                };
            }
        }

        public bool IsExpertQuery
        {
            get
            {
                if (Terms.Any(t => t.Intersect(ExpertSymbols).Any()))
                {
                    return true;
                }
                return false;
            }
        }

        public string GetSimpleQueryStringWithFuzziness()
        {
            if (IsExpertQuery)
            {
                return OriginalQuery;
            }

            StringBuilder output = new StringBuilder();
            var termFuzziness = 0;
            var maxFuzinessPerTerm = 3;
            foreach (var term in Terms)
            {
                if (term.Any(char.IsDigit))
                {
                    //leave this term as-is - its a date, or a reference number
                    output.Append(" " + term);
                }
                else
                {
                    termFuzziness = Math.Min(term.Length / 4, maxFuzinessPerTerm);

                    output.Append($" {term}~{termFuzziness}");
                }
            }

            return output.ToString().Trim();
        }
    }
}
