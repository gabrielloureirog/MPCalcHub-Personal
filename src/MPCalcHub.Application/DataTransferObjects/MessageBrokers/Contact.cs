namespace MPCalcHub.Application.DataTransferObjects.MessageBrokers;

public record Contact(
    Guid Id,
    DateTime CreatedAt,
    Guid CreatedBy,
    DateTime? UpdatedAt,
    Guid? UpdatedBy,
    bool Removed,
    DateTime? RemovedAt,
    Guid? RemovedBy,
    string Name, 
    int DDD, 
    string PhoneNumber, 
    string Email
);
