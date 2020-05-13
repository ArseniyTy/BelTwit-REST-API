using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BelTwit_REST_API.Models;

namespace BelTwit_REST_API.Logging
{
    //добавляет к объекту ILoggerFactory метод расширения AddDatabase, 
    //который будет добавлять наш провайдер логгирования.
    public static class DatabaseLoggerExtensions
    {
        public static ILoggerFactory AddDatabase(this ILoggerFactory factory, BelTwitContext context)
        {
            factory.AddProvider(new DatabaseLoggerProvider(context));
            return factory;
        }
    }
}
