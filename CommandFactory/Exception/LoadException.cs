namespace CommandFactory.Exception
{
  internal class LoadException : System.Exception
  {
    public LoadException()
    {
    }

    public LoadException(string? message) : base(message)
    {
    }

    public LoadException(string? message, System.Exception? innerException) : base(message, innerException)
    {
    }
  }
}