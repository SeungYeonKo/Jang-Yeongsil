using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class UnityWebRequestExtensions
{
    public static Task<UnityWebRequest> SendWebRequestAsync(this UnityWebRequest webRequest)
    {
        var tcs = new TaskCompletionSource<UnityWebRequest>();

        webRequest.SendWebRequest().completed += operation =>
        {
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                tcs.SetException(new UnityWebRequestException(webRequest.error));
            }
            else
            {
                tcs.SetResult(webRequest);
            }
        };

        return tcs.Task;
    }
}

public class UnityWebRequestException : System.Exception
{
    public UnityWebRequestException(string message) : base(message) { }
}
