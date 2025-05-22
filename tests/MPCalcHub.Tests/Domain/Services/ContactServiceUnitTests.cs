using Moq;
using Xunit;
using MPCalcHub.Application.Interfaces;
using MPCalcHub.Domain.Entities;
using MPCalcHub.Domain.Interfaces;
using MPCalcHub.Domain.Services;
using MPCalcHub.Tests.Shared.Fixtures.Entities;
using EN = MPCalcHub.Domain.Entities;

using MPCalcHub.Domain.Interfaces.Infrastructure;

public class ContactServiceUnitTests
{
    private readonly Mock<IContactRepository> _contactRepositoryMock;
    private readonly Mock<IStateDDDRepository> _stateDDDRepositoryMock;

    private readonly Mock<IStateDDDService> _stateDDDServiceMock;
    private readonly Mock<EN.UserData> _userDataMock;
    private readonly IContactService _contactService;

    public ContactServiceUnitTests()
    {
        _contactRepositoryMock = new Mock<IContactRepository>();
        _stateDDDRepositoryMock = new Mock<IStateDDDRepository>();
        _stateDDDServiceMock = new Mock<IStateDDDService>();
        _userDataMock = new Mock<EN.UserData>();

        _contactService = new ContactService(
            _contactRepositoryMock.Object,
            _userDataMock.Object,
            _stateDDDServiceMock.Object
        );
    }

    [Fact]
    public async Task Create_ValidContact_ShouldCallRepositoryOnce()
    {
        // Arrange
        var contact = ContactFixtures.CreateAs_Base();

        // Mock do serviço de DDD, configurando para retornar um DDD válido
        _stateDDDServiceMock.Setup(service => service.GetByDDDAsync(It.IsAny<int>()))
                            .ReturnsAsync(new StateDDD { DDD = contact.DDD, State = "São Paulo", Region = "Sudeste" });

        // Mock do repositório de contatos, configurando para adicionar o contato
        _contactRepositoryMock.Setup(repo => repo.Add(It.IsAny<Contact>()))
                              .ReturnsAsync(contact);

        // Act
        var result = await _contactService.Add(contact);

        // Assert
        _contactRepositoryMock.Verify(repo => repo.Add(It.IsAny<Contact>()), Times.Once); // Verifica se o método Add foi chamado uma vez
        _stateDDDServiceMock.Verify(service => service.GetByDDDAsync(It.IsAny<int>()), Times.Once); // Verifica se o GetByDDDAsync foi chamado uma vez
        Assert.NotNull(result);
        Assert.Equal(contact.Name, result.Name);
    }

    [Fact]
    public async Task Update_ExistingContact_ShouldCallRepositoryOnce()
    {
        // Arrange
        var contact = ContactFixtures.CreateAs_Base();
        contact.Name = "Updated Name";

        // Mock do serviço de DDD, configurando para retornar um DDD válido
        _stateDDDServiceMock.Setup(service => service.GetByDDDAsync(It.IsAny<int>()))
                            .ReturnsAsync(new StateDDD { DDD = contact.DDD, State = "São Paulo", Region = "Sudeste" });

        _contactRepositoryMock.Setup(repo => repo.Update(It.IsAny<Contact>()))
                              .ReturnsAsync(contact);

        // Act
        var result = await _contactService.Update(contact);

        // Assert
        _contactRepositoryMock.Verify(repo => repo.Update(It.IsAny<Contact>()), Times.Once);
        Assert.Equal("Updated Name", result.Name);
    }

    [Fact]
    public async Task Delete_ValidId_ShouldCallRepositoryOnce()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        _contactRepositoryMock.Setup(repo => repo.Delete(contactId))
                              .Returns(Task.CompletedTask);

        // Act
        await _contactService.Delete(contactId);

        // Assert
        _contactRepositoryMock.Verify(repo => repo.Delete(contactId), Times.Once);
    }
}
