namespace MPCalcHub.Application.DataTransferObjects.MessageBrokers;

public record BasicContact(
    string Name, 
    int DDD, 
    string PhoneNumber, 
    string Email
);