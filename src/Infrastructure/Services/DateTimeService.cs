using DarInternet.Application.Common.Interfaces;
using System;

namespace DarInternet.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
