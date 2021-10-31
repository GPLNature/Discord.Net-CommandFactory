namespace CommandFactory.Exception
{
  internal class ParameterMappingException : System.Exception
  {
    public ParameterMappingException()
    {
    }

    public ParameterMappingException(string? message) : base(message)
    {
    }

    public ParameterMappingException(string? message, System.Exception? innerException) : base(message, innerException)
    {
    }
  }
}