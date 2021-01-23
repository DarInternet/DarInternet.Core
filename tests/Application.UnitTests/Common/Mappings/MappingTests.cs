using AutoMapper;
using DarInternet.Application.Common.Mappings;
using DarInternet.Application.Features.Organizations.SharedDto;
using DarInternet.Application.Features.Tickets.SharedDto;
using DarInternet.Domain.Entities;
using NUnit.Framework;
using System;
using System.Runtime.Serialization;

namespace DarInternet.Application.UnitTests.Common.Mappings
{
    public class MappingTests
    {
        private readonly IConfigurationProvider _configuration;
        private readonly IMapper _mapper;

        public MappingTests()
        {
            _configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = _configuration.CreateMapper();
        }

        [Test]
        public void ShouldHaveValidConfiguration()
        {
            _configuration.AssertConfigurationIsValid();
        }
        
        [Test]
        [TestCase(typeof(Organization), typeof(OrganizationDto))]
        [TestCase(typeof(Conversation), typeof(TicketDto))]
        [TestCase(typeof(ConversationMessage), typeof(TicketMessageDto))]
        public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
        {
            var instance = GetInstanceOf(source);

            _mapper.Map(instance, source, destination);
        }

        private object GetInstanceOf(Type type)
        {
            if (type.GetConstructor(Type.EmptyTypes) != null)
                return Activator.CreateInstance(type);

            // Type without parameterless constructor
            return FormatterServices.GetUninitializedObject(type);
        }
    }
}
