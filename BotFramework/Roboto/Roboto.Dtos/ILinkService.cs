namespace Roboto.Dtos
{
    public interface ILinkService
    {
        Link Generate(string endpointName, string controller, object? routeValues, string rel, string method);
    }
}