namespace OnliveConstants.Communication;

public enum RequestAction
{
    /// <summary>
    /// Used to indicate that a request could not be decoded
    /// </summary>
    Unknown,
    /// <summary>
    /// Used for a server request, providing board data to a client
    /// </summary>
    SendBoard,
    /// <summary>
    /// Used for a client request, asking to switch the value of a cell on the board
    /// </summary>
    SwitchCells,
    /// <summary>
    /// Used to indicate to the server the current position of a client
    /// </summary>
    SendCurrentPosition,
}
