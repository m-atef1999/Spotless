
namespace Spotless.Application.Interfaces
{
    public interface IPaginationService
    {
        int NormalizePageSize(int? requestedPageSize);
        int GetDefaultPageNumber();
    }
}
