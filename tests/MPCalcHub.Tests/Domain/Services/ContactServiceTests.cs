using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MPCalcHub.Api.Controllers;
using EN = MPCalcHub.Domain.Entities;
using MPCalcHub.Domain.Interfaces;
using MPCalcHub.Domain.Interfaces.Infrastructure;
using MPCalcHub.Domain.Services;
using MPCalcHub.Infrastructure.Data;
using MPCalcHub.Infrastructure.Data.Repositories;
using MPCalcHub.Tests.Shared.Fixtures.Entities;
using MPCalcHub.Tests.Shared.Fixtures.Utils;
using MPCalcHub.Application.DataTransferObjects;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using MPCalcHub.Application.Interfaces;
using MPCalcHub.Application.Services;
using MPCalcHub.Application.Mappings;
using MPCalcHub.Tests.Shared.Fixtures;
using MPCalcHub.Tests.Shared.Fixtures.DataTransferObjects;

namespace MPCalcHub.Tests.Integration
{
    public class ContactControllerIntegrationTests : IAsyncLifetime
    {
        private readonly ApplicationDBContext _context;
        private readonly IContactRepository _contactRepository;
        private readonly IStateDDDRepository _stateDDDRepository;
        private readonly IStateDDDService _stateDDDService;
        private readonly IContactService _contactService;
        private readonly ContactController _controller;
        private readonly IContactApplicationService _contactApplicationService;
        private readonly IMapper _mapper;

        private readonly EN.UserData _userData;

        public ContactControllerIntegrationTests()
        {
            // Configura o DbContext em memória
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDBContext(options);
            // Configura o AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ContactMapper>();
            });
            _mapper = config.CreateMapper();

            // Configura as dependências
            _userData = UserDataFixtures.CreateAs_Base();
            _contactRepository = new ContactRepository(_context);
            _stateDDDRepository = new StateDDDRepository(_context);
            _stateDDDService = new StateDDDService(_stateDDDRepository, _userData);
            _contactService = new ContactService(_contactRepository, _userData, _stateDDDService);
            _contactApplicationService = new ContactApplicationService(_contactService, _mapper);
            var logger = NullLogger<ContactController>.Instance;
            // Instancia o ContactController diretamente
            _controller = new ContactController(logger, _contactApplicationService);
        }

        public async Task DisposeAsync()
        {
            await _context.Database.EnsureDeletedAsync();
            _context.Dispose();
        }

        [Fact]
        public async Task ShouldInsertContact()
        {
            // Arrange
            var contact = BasicContactFixtures.CreateAs_Base();
            // Act
            var result = await _controller.Create(contact);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedContact = Assert.IsType<Contact>(okResult.Value);
            Assert.NotNull(returnedContact);
            Assert.Equal(contact.Name, returnedContact.Name);
            Assert.Equal(contact.DDD, returnedContact.DDD);
            Assert.Equal(contact.PhoneNumber, returnedContact.PhoneNumber);
        }

        [Fact]
        public async Task ShouldInsertContact_InvalidDDD_ExpectedThrow()
        {
            // Arrange
            var contact = BasicContactFixtures.CreateAs_Base();
            contact.DDD = 100;
            // Act
            var result = await _controller.Create(contact);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("DDD inválido e/ou não existe.", badRequestResult.Value);
        }

        [Fact]
        public async Task ShouldUpdateContact()
        {
            // Arrange
            await ShouldInsertContact();
            var contacts = _contactService.GetAll();
            var contact = _mapper.Map<Contact>(contacts.FirstOrDefault());

            Assert.NotNull(contact); // Garante que existe um contato para atualizar

            // Atualiza o contato
            contact.Name = "Updated Name";

            // Act
            var result = await _controller.Update(contact);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var updatedContact = Assert.IsType<Contact>(okResult.Value);
            Assert.NotNull(updatedContact);
            Assert.Equal("Updated Name", updatedContact.Name);
        }

        [Fact]
        public async Task ShouldFindByDDD()
        {
            // Arrange
            var dddFilter = 12;
            var contact1 = ContactFixtures.CreateAs_Base();
            var contact2 = ContactFixtures.CreateAs_Base();
            contact2.DDD = dddFilter;

            await _context.AddRangeAsync(contact1, contact2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.FindByDDD(dddFilter);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var contacts = Assert.IsType<List<Contact>>(okResult.Value);
            Assert.NotEmpty(contacts);
            Assert.Single(contacts); // Deve encontrar 1 contato com DDD 12
            Assert.All(contacts, contact => Assert.Equal(dddFilter, contact.DDD));
        }

        public async Task InitializeAsync()
        {
            // Limpa o banco de dados e cria um novo
            await _context.Database.EnsureDeletedAsync();

            // Carrega os dados iniciais (seeds)
            var states = StateDDDFixtures.GenerateAllStateDDD();
            await _context.AddRangeAsync(states);

            await _context.SaveChangesAsync();
        }
    }
}