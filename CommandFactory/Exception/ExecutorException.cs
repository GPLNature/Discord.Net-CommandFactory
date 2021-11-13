namespace CommandFactory.Exception
{
  internal class TooManyExecutorException : System.Exception
  {
    public TooManyExecutorException()
    {
    }

    public TooManyExecutorException(string? message) : base(message)
    {
    }

    public TooManyExecutorException(string? message, System.Exception? innerException) : base(message, innerException)
    {
    }
  }
  
  internal class EmptyExecutorException : System.Exception
  {
    public EmptyExecutorException()
    {
    }

    public EmptyExecutorException(string? message) : base(message)
    {
    }

    public EmptyExecutorException(string? message, System.Exception? innerException) : base(message, innerException)
    {
    }
  }
  
  internal class ReceivedDataException: System.Exception
  {
    public ReceivedDataException(string? message) : base(message)
    {
    }

    public ReceivedDataException(string? message, System.Exception? innerException) : base(message, innerException)
    {
    }
  }
}