using System.Collections.Generic;
using System.Linq;

namespace Antiban
{
    public class Antiban
    {
        public Antiban()
        {
            EventMessages = new List<EventMessage>();
        }

        private List<EventMessage> EventMessages { get; set; }
        
        /// <summary>
        /// Добавление сообщений в систему, для обработки порядка сообщений
        /// </summary>
        /// <param name="eventMessage"></param>
        public void PushEventMessage(EventMessage eventMessage)
        {
            //TODO:
            if (EventMessages.Count == 0)
            {
                EventMessages.Add(eventMessage);
            }
            else
            {
                if (eventMessage.Priority == 1)
                {
                    //TODO: case 1
                    var priority1 = EventMessages
                        .Where(x => x.Phone.Equals(eventMessage.Phone) && x.Priority == 1)
                        .OrderByDescending(x => x.DateTime)
                        .FirstOrDefault();

                    if (priority1 != null)
                    {
                        if (eventMessage.DateTime < priority1.DateTime.AddDays(1))
                        {
                            //TODO: чтобы работал по unit test
                            //eventMessage.DateTime = eventMessage.DateTime.AddDays(1);

                            //TODO: чтобы работал по описанию
                            //+ 24 часа (приоритет = 1) по этому номеру
                            eventMessage.DateTime = priority1.DateTime.AddDays(1);
                        }
                    }

                    var priority0 = EventMessages
                        .Where(x => x.Phone.Equals(eventMessage.Phone) && x.Priority == 0)
                        .OrderByDescending(x => x.DateTime)
                        .FirstOrDefault();

                    if (priority0 != null)
                    {
                        if (eventMessage.DateTime < priority0.DateTime.AddMinutes(1))
                        {
                            eventMessage.DateTime = priority0.DateTime.AddMinutes(1);
                        }
                    }
                }
                else
                {
                    //TODO: case 2
                    var priority0 = EventMessages
                        .Where(x => x.Phone.Equals(eventMessage.Phone) && x.DateTime <= eventMessage.DateTime)
                        .OrderByDescending(x => x.DateTime)
                        .FirstOrDefault();

                    if (priority0 != null)
                    {
                        if (eventMessage.DateTime < priority0.DateTime.AddMinutes(1))
                        {
                            eventMessage.DateTime = priority0.DateTime.AddMinutes(1);
                        }
                    }
                }

                //TODO: case 3
                var lastEvent = EventMessages.Count == 0 ? 
                    EventMessages.FirstOrDefault()
                    :
                    EventMessages.Where(x => x.DateTime < eventMessage.DateTime)
                        .OrderByDescending(x=>x.DateTime).FirstOrDefault()
                    ;

                if (eventMessage.DateTime < lastEvent?.DateTime.AddSeconds(10))
                {
                    eventMessage.DateTime = lastEvent.DateTime.AddSeconds(10);
                    var checkDate = true;
                    while(checkDate)
                    {
                        var check = EventMessages.FirstOrDefault(x => x.DateTime == eventMessage.DateTime);
                        if (check != null)
                            eventMessage.DateTime = check.DateTime.AddSeconds(10);
                        else
                            checkDate = false;
                    }
                }

                EventMessages.Add(eventMessage);
                return;
            }
        }

        /// <summary>
        /// Вовзращает порядок отправок сообщений
        /// </summary>
        /// <returns></returns>
        public List<AntibanResult> GetResult()
        {
            //TODO
            //Example
            var result = new List<AntibanResult>();
            //for (int i = 0; i < 10; i++)
            //{
            //    result.Add(new AntibanResult());
            //}
            foreach (var item in EventMessages.OrderBy(x=>x.DateTime).ToList())
            {
                result.Add(new AntibanResult
                {
                    EventMessageId = item.Id,
                    SentDateTime = item.DateTime
                });
            }       
            return result;
        }
    }
}
