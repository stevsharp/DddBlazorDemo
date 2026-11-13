namespace DddBlazorDemo.Application.ReadModels;

public sealed record CustomerListItemDto(
    Guid Id,
    string Name,
    string VatNumber,
    string Email,
    string? Phone,
    string City,
    string CountryCode);
