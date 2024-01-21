namespace SafeShare.ProxyApi.Container.Interfaces;

public interface IRequestConfigurationProxyService
{
    string
    GetApiKey();

    string
    GetBaseAddrOfMainApi();

    string
    GetClient();
}