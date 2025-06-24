using OnliveConstants.Requests;

namespace OnliveConstants.Communication;

public class RequestDecoder
{
    public static RequestAction DecodeRequestAction(string request)
    {
        if (request.Length < 3) return RequestAction.Unknown;

        var requestActionStr = request.Substring(0, 2);

        try
        {
            return (RequestAction)Convert.ToInt32(requestActionStr);
        }
        catch (FormatException)
        {
            return RequestAction.Unknown;
        }
        catch (OverflowException)
        {
            return RequestAction.Unknown;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return RequestAction.Unknown;
        }
    }

    /// <summary>
    /// Decodes a SendBoard request. Such a request's payload is composed of a collection of positions, representing
    ///     the positions of active cells
    /// </summary>
    /// <example>Request payload: "1;4|21;12|12;-8"</example>
    /// <returns>The SendBoard data associated with the request</returns>
    /// <remarks>Returns null if the request could not be decoded</remarks>
    public static SendBoardRequest? DecodeSendBoardRequest(string request)
    {
        if (request.Length < 3) return null;

        request = request.Substring(3);

        IEnumerable<Position> positions = request.Split("|").Select(pair => DecodePosition(pair) ?? Position.Zero);

        return new() { ActiveCells = positions };
    }

    /// <summary>
    /// Decodes a SwitchCell request. Such a request's payload is composed of a single position
    /// </summary>
    /// <example>Request payload: "1;4"</example>
    /// <returns>The SwitchCase data associated with the request</returns>
    /// <remarks>Returns null if the request could not be decoded</remarks>
    public static SwitchCellRequest? DecodeSwitchCellRequest(string request)
    {
        if (request.Length < 3) return null;

        request = request.Substring(3);

        var position = DecodePosition(request);
        if (!position.HasValue) return null;

        return new() { SwitchedCell = position.Value };
    }

    private static Position? DecodePosition(string stringPosition)
    {
        var xyPair = stringPosition.Split(";");

        if (xyPair.Length != 2) return null;

        return new Position(Convert.ToInt32(xyPair[0]), Convert.ToInt32(xyPair[1]));
    }
}
