using System;
using Newtonsoft.Json;

namespace RabbitMQDownloadProgramProducerConsumer
{
    public static class Deserializer
    {
        public static void Deserialize<T>(string json, Action<T> done, Action<System.Exception> fail)
        {
            try
            {
                done(JsonConvert.DeserializeObject<T>(json));
            }
            catch (System.Exception ex)
            {
                fail(ex);
            }
        }

        public static void Deserialize(string json, Type type, Action<object> done, Action<System.Exception> fail)
        {
            try
            {
                done(JsonConvert.DeserializeObject(json, type));
            }
            catch (System.Exception ex)
            {
                fail(ex);
            }
        }
    }
}