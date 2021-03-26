using Katsuretsu.Application.Common.Interfaces;
using System;

namespace Katsuretsu.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
