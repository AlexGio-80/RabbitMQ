using System;

namespace RabbitMQConsumerTest
{
    public class MessageDeserializer
    {
        private readonly Func<Type> _createCallBack;

        public MessageDeserializer(Func<Type> createCallBack)
        {
            _createCallBack = createCallBack;
        }

        public void Create(Message baseMessage, Action<IMessage> done, Action<System.Exception> fail)
        {
            var messageType = _createCallBack();

            Deserializer.Deserialize(
                baseMessage.Payload,
                messageType,
                message =>
                {
                    var completeMessage = (IMessage) message;
                    completeMessage.TimeStampUtc = baseMessage.TimeStampUtc;
                    completeMessage.TimeStamp = baseMessage.TimeStamp;

                    done(completeMessage);
                },
                fail);
        }
    }
}