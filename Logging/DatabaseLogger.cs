using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BelTwit_REST_API.Models;

namespace BelTwit_REST_API.Logging
{
    public class DatabaseLogger : ILogger
    {
        private BelTwitContext _db;
        
        public DatabaseLogger(BelTwitContext context)
        {
            _db = context;
        }


        //область видимости для логгера
        public IDisposable BeginScope<TState>(TState state) { return null;  }

        //доступен ли логгер для использования
        public bool IsEnabled(LogLevel logLevel) {  return true;  }


        //formatter - проста нейкая функція, которая возвращает сообщение пользователя 
        // + состояніе і текст ошібкі (еслі мы таковые передалі)
        public void Log<TState>(LogLevel logLevel, EventId eventId, 
            TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var log = new Log
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Now,
                Action = formatter(state, exception),
                LogLevel = logLevel.ToString()
            };
            _db.Logs.Add(log);
            _db.SaveChanges();  //ORDER BY Date чтобы нормально просматрівать
        }

        /*
         * Можно добавіть метод LogUser - который будет о user інфу в нужном формате запісывать (он вызывает обычный log.info)
         * Можно добавіть поля в модель: Adress(api/created :ex), HttpMethod(Post/Get...), 
         */

    }
}
