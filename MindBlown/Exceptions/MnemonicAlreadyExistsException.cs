namespace MindBlown.Exceptions
{
    public class MnemonicAlreadyExistsException : Exception
    {
        public MnemonicAlreadyExistsException(string message) : base(message) { }
    }
}