using System;

public class LocationNotAuthorizedException : Exception
{
    public LocationNotAuthorizedException()
    {
    }

    public LocationNotAuthorizedException(string message) : base(message)
    {
    }

    public LocationNotAuthorizedException(string message, Exception inner) : base(message, inner)
    {
    }
}