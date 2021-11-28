using HousingRegisterApi.V1.Domain.Report.Internal;
using System;

namespace HousingRegisterApi.V1.Boundary.Request
{
    public class InternalReportRequest
    {
        public InternalReportType ReportType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
