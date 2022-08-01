using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HousingRegisterApi.V1.Infrastructure.Search
{
    public class ApplicationSearchQueryParser
    {
        const string ExpertSymbols = "+|\"*()~";

        public string OriginalQuery { get; set; }
        public List<string> Terms { get; set; }

        public List<string> MatchedTerms { get; set; }

        public List<DateTime> Dates { get; set; }

        public List<string> ReferenceNumbers { get; set; }

        public List<string> NINOs { get; set; }

        public List<Guid> ApplicationIds { get; set; }

        public ApplicationSearchQueryParser(string inputQuery)
        {
            Terms = inputQuery.Split(" ").ToList();
            Dates = new List<DateTime>();
            ReferenceNumbers = new List<string>();
            NINOs = new List<string>();
            MatchedTerms = new List<string>();
            ApplicationIds = new List<Guid>();
            OriginalQuery = inputQuery;
            if (!string.IsNullOrWhiteSpace(inputQuery))
            {
                Terms = inputQuery.Split(" ").ToList();
            }
            else
            {
                Terms = new List<string>();
            }
        }

        public ApplicationSearchQueryParser CaptureDates()
        {
            foreach (var term in Terms.ToArray())
            {
                if (term.Contains("-") || term.Contains("/"))
                {
                    DateTime parsedDate = DateTime.MinValue;

                    //Potential date
                    if (DateTime.TryParse(term, out parsedDate))
                    {
                        Dates.Add(parsedDate);
                        Terms.Remove(term);
                        MatchedTerms.Add(term);
                    }
                }
            }
            return this;
        }

        public ApplicationSearchQueryParser CaptureReferenceNumbers(bool capture)
        {
            foreach (var term in Terms.ToArray())
            {
                if (long.TryParse(term, System.Globalization.NumberStyles.HexNumber, null, out _))
                {
                    ReferenceNumbers.Add(term);

                    if (capture)
                    {
                        Terms.Remove(term);
                    }
                    MatchedTerms.Add(term);
                }
            }
            return this;
        }

        public ApplicationSearchQueryParser CaptureNINO(bool capture)
        {
            foreach (var term in Terms.ToArray())
            {
                if (term.Length >= 3)
                {
                    if (char.IsLetter(term[0]) && char.IsLetter(term[1]) && char.IsDigit(term[2]))
                    {
                        NINOs.Add(term);

                        if (capture)
                        {
                            Terms.Remove(term);
                        }
                        MatchedTerms.Add(term);
                    }
                }
            }
            return this;
        }

        public ApplicationSearchQueryParser CaptureGuids()
        {
            foreach (var term in Terms.ToArray())
            {
                if (term.Length >= 24)
                {
                    Guid parsedGuid = Guid.Empty;
                    if (Guid.TryParse(term, out parsedGuid))
                    {
                        ApplicationIds.Add(parsedGuid);
                        Terms.Remove(term);
                        MatchedTerms.Add(term);
                    }
                }
            }
            return this;
        }

        public ApplicationSearchQueryParser RemoveMatched()
        {
            Terms = Terms.Except(MatchedTerms).ToList();
            return this;
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

        public ApplicationSearchSemiStructuredQuery Query { get
            {
                return new ApplicationSearchSemiStructuredQuery
                {
                    ApplicationIds = ApplicationIds,
                    Terms = Terms,
                    Dates = Dates,
                    NINOs = NINOs,
                    OriginalQuery = OriginalQuery,
                    ReferenceNumbers = ReferenceNumbers
                };
            }
        }

        

    }
}
