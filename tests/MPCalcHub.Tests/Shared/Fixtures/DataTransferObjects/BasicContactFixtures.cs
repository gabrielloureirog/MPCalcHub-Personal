using Bogus;
using MPCalcHub.Application.DataTransferObjects;
using MPCalcHub.Tests.Shared.Fixtures.Utils;

namespace MPCalcHub.Tests.Shared.Fixtures.DataTransferObjects;

public sealed class BasicContactFixtures : BaseFixtures<BasicContact>
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

    public BasicContactFixtures() : base() { }

    public static BasicContact GenerateUser()
    {
        var faker = Faker
            .RuleFor(u => u.Name, f => f.Person.FullName)
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.DDD, f => f.PickRandom(ValidBrazilianDDDs))
            .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber("9########"));

        return faker.Generate();
    }

    public static BasicContact CreateAs_Base()
    {
        var contact = GenerateUser();

        return contact;
    }

    public static BasicContact CreateAs_InvalidName()
    {
        var contact = CreateAs_Base();
        contact.Name = string.Empty;

        return contact;
    }

    public static BasicContact CreateAs_InvalidEmail()
    {
        var contact = CreateAs_Base();
        contact.Email = FakerDefault.Random.String2(2, 2);

        return contact;
    }

    public static BasicContact CreateAs_InvalidPhoneNumber()
    {
        var contact = CreateAs_Base();
        contact.PhoneNumber = FakerDefault.Random.String2(2, 2);

        return contact;
    }

    public static BasicContact CreateAs_InvalidDDD()
    {
        var contact = CreateAs_Base();
        contact.DDD = FakerDefault.Random.Int(2, 2);

        return contact;
    }
}