// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>", Scope = "member", Target = "~M:HousingRegisterApi.V1.Infrastructure.TokenGenerator.ValidateToken(System.String,System.Collections.Generic.IEnumerable{System.Security.Claims.Claim}@)~System.Boolean")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>", Scope = "member", Target = "~M:HousingRegisterApi.V1.Services.BedroomCalculatorService.Calculate(HousingRegisterApi.V1.Domain.Application)~System.Nullable{System.Int32}")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>", Scope = "member", Target = "~M:HousingRegisterApi.V1.UseCase.RecalculateBedroomsUseCase.Execute")]
[assembly: SuppressMessage("Usage", "CA2214:Do not call overridable methods in constructors", Justification = "<Pending>", Scope = "member", Target = "~M:HousingRegisterApi.V1.Functions.BaseFunction.#ctor")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>", Scope = "member", Target = "~P:HousingRegisterApi.V1.Domain.Report.NovaletExportDataRow.AutoBidPref_MobilityStandard")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>", Scope = "member", Target = "~P:HousingRegisterApi.V1.Domain.Report.NovaletExportDataRow.AutoBidPref_WheelChairStandard")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>", Scope = "member", Target = "~P:HousingRegisterApi.V1.Domain.Report.NovaletExportDataRow.AutoBidPref_AdaptedStandard")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "<Pending>", Scope = "member", Target = "~P:HousingRegisterApi.V1.Domain.Report.ExportFile.Data")]
[assembly: SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "<Pending>", Scope = "member", Target = "~F:HousingRegisterApi.V1.Domain.Report.CsvData.Headers")]
[assembly: SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "<Pending>", Scope = "member", Target = "~F:HousingRegisterApi.V1.Domain.Report.CsvData.DataRows")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>", Scope = "member", Target = "~M:HousingRegisterApi.V1.Gateways.ApplicationActivityGateway.GetActivities(System.Guid)~System.Threading.Tasks.Task{System.Collections.Generic.List{Hackney.Shared.ActivityHistory.Boundary.Response.ActivityHistoryResponseObject}}")]
[assembly: SuppressMessage("Performance", "CA1825:Avoid zero-length array allocations.", Justification = "<Pending>", Scope = "type", Target = "~T:HousingRegisterApi.V1.Controllers.ApplicationsApiController")]
[assembly: SuppressMessage("Performance", "CA1825:Avoid zero-length array allocations.", Justification = "<Pending>", Scope = "type", Target = "~T:HousingRegisterApi.V1.Controllers.AuthApiController")]
[assembly: SuppressMessage("Performance", "CA1825:Avoid zero-length array allocations.", Justification = "<Pending>", Scope = "type", Target = "~T:HousingRegisterApi.V1.Controllers.ExceptionController")]
[assembly: SuppressMessage("Performance", "CA1825:Avoid zero-length array allocations.", Justification = "<Pending>", Scope = "type", Target = "~T:HousingRegisterApi.V1.Controllers.HealthCheckController")]
[assembly: SuppressMessage("Performance", "CA1825:Avoid zero-length array allocations.", Justification = "<Pending>", Scope = "type", Target = "~T:HousingRegisterApi.V1.Controllers.ReportingApiController")]
