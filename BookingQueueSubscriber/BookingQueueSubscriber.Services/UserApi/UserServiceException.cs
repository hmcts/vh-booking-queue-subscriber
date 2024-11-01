using System.Runtime.Serialization;

namespace BookingQueueSubscriber.Services.UserApi
{
    [Serializable]
    public class UserServiceException : Exception
    {
        public string Reason { get; set; }
        public UserServiceException(string message, string reason) : base($"{message}: {reason}")
        {
            Reason = reason;
        }
        
        protected UserServiceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Reason = info.GetString(ExceptionReason.Reason);
        }

        public UserServiceException()
        {
        }
        
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(ExceptionReason.Info);
            }

            info.AddValue(ExceptionReason.Reason, Reason);

            // MUST call through to the base class to let it save its own state
            base.GetObjectData(info, context);
        }
        
        private static class ExceptionReason
        {
            public const string Reason = "Reason";
            public const string Info = "info";
        }
    }
}
