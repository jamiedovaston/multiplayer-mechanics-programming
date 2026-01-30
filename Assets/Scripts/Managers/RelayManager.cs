using System.Threading.Tasks;

public class RelayManager : Singleton<RelayManager>
{
    public string joinCode { get; private set; } = string.Empty;
    public string helloString = "Hello String";

    public async Task<bool> StartHost()
    {
        joinCode = await NetworkServices.StartHostWithRelay(2, "udp");
        return !string.IsNullOrEmpty(joinCode);
    }

    public async Task<bool> StartClient(string code)
    {
        bool success = await NetworkServices.StartClientWithRelay(code, "udp");
        if (success) joinCode = code;
        return success;
    }
}

