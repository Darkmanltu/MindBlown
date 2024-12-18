namespace MindBlown.Exceptions
{
    public class MnemonicServiceException : Exception
    {
        public MnemonicServiceException(string message) : base(message) { }
        public MnemonicServiceException(string message, Exception innerException) : base(message, innerException) { }
    }
}
