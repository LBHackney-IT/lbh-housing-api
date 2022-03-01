namespace HousingRegisterApi.V1.Domain.Report.Novalet
{
    public class NovaletExportDataRowExtended : NovaletExportDataRow
    {
        public NovaletExportDataRowExtended(Application application) : base(application)
        {
                Url = "https://housing-register.hackney.gov.uk/applications/view/" + application.Id;
        }

#pragma warning disable CA1056 // Uri properties should not be strings
        public string Url { get; set; }
#pragma warning restore CA1056 // Uri properties should not be strings
    }
}
