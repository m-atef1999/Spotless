using Microsoft.AspNetCore.Mvc;

namespace Spotless.API.Attributes
{
    public class AuditAttribute : TypeFilterAttribute
    {
        public AuditAttribute() : base(typeof(Filters.AuditActionFilter))
        {
        }
    }
}
