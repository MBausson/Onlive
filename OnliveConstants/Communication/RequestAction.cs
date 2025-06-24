namespace OnliveConstants.Communication;

public enum RequestAction
{
    /// <summary>
    /// Used for a server request, providing board data to a client
    /// </summary>
    SendBoard,
    /// <summary>
    /// Used for a client request, asking to switch the value of a cell on the board
    /// </summary>
    SwitchCell,
    /// <summary>
    /// Used to indicate that a request could not be decoded
    /// </summary>
    Unknown
}
