using Bogus;
using MPCalcHub.Domain.Entities;
using MPCalcHub.Tests.Shared.Fixtures.Utils;
using EN = MPCalcHub.Domain.Entities;
using DTO = MPCalcHub.Application.DataTransferObjects;


namespace MPCalcHub.Tests.Shared.Fixtures.Entities;

public sealed class ContactFixtures : BaseFixtures<EN.Contact>
{
    private static readonly int[] ValidBrazilianDDDs = new[]
  {
        11, 12, 13, 14, 15, 16, 17, 18, 19,
        21, 22, 24,
        27, 28,
        31, 32, 33, 34, 35, 37, 38,
        41, 42, 43, 44, 45, 46,
        47, 48, 49,
        51, 53, 54, 55,
        61,
        62, 63, 64,
        65, 66,
        67,
        68,
        69,
        71, 73, 74, 75, 77,
        79,
        81, 82, 83, 84, 85, 86, 87, 88, 89,
        91, 92, 93, 94, 95, 96, 97, 98, 99
    };

    public static EN.Contact GenerateContact()
    {
        var faker = new Faker<EN.Contact>("pt_BR")
            .RuleFor(u => u.Name, f => f.Person.FullName)
            .RuleFor(u => u.Email, f => f.Person.Email)
            .RuleFor(u => u.DDD, f => f.PickRandom(ValidBrazilianDDDs))
            .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber("#########"));

        return faker.Generate();
    }

    public static EN.Contact CreateAs_Base()
    {
        var faker = new Faker<EN.Contact>("pt_BR")
            .RuleFor(u => u.Name, f => f.Person.FullName)
            .RuleFor(u => u.Email, f => f.Person.Email)
            .RuleFor(u => u.DDD, f => f.PickRandom(ValidBrazilianDDDs))
            .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber("#########"));

        var contact = faker.Generate();
        contact.PrepareToInsert(Guid.NewGuid());

        return contact;
    }

    public static DTO.BasicContact CreateAs_BaseDTO()
    {
        var faker = new Faker<DTO.BasicContact>("pt_BR")
            .RuleFor(u => u.Name, f => f.Person.FullName)
            .RuleFor(u => u.Email, f => f.Person.Email)
            .RuleFor(u => u.DDD, f => f.PickRandom(ValidBrazilianDDDs))
            .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber("#########"));

        var contact = faker.Generate();

        return contact;
    }

    public static DTO.Contact CreateAs_DTO()
    {
        var faker = new Faker<DTO.Contact>("pt_BR")
            .RuleFor(u => u.Name, f => f.Person.FullName)
            .RuleFor(u => u.Email, f => f.Person.Email)
            .RuleFor(u => u.DDD, f => f.Random.Int(10, 99))
            .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber("#########"));

        var contact = faker.Generate();

        return contact;
    }
}