using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Passflow.Presentation.OperationFilters;
internal class MethodNameAsOperationIdFilter : IOperationFilter
{
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		var actionMethod = context.MethodInfo;
		operation.OperationId = actionMethod.Name;
	}
}