namespace Roboto.Models.Dto
{
    public interface ILinkService
    {
        Link Generate(string endpointName, string controller, object? routeValues, string rel, string method);
    }
}