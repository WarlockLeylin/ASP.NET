using Midterm.Domain.Enums;

namespace Midterm.DTOs;

public record CreateBugReportRequest(
    string Title,
    SeverityLevel SeverityLevel
);

public record CreateFeatureRequestRequest(
    string Title,
    double EstimatedHours
);