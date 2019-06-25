using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.AspNetCore.Routing;
using Sedio.Logic.Data;
using Sedio.Web.Execution;

namespace Sedio.Server.Api.Rest
{
    public abstract class ResourceController : ExecutionHostController
    {
        protected string BranchId => GetOptionalBranchId() ?? throw new InvalidOperationException("BranchId not present");

        protected override IReadOnlyDictionary<string, object> OnGetExecutionItems()
        {
            var result = new Dictionary<string,object>();

            var branchId = GetOptionalBranchId();

            if (branchId != null)
            {
                result.Add(ModelBranchExtensions.BranchIdKey, branchId);
            }

            return result;
        }

        private string? GetOptionalBranchId()
        {
            if (HttpContext.GetRouteData().Values.TryGetValue("branchId", out var value))
            {
                return value.ToString();
            }

            return null;
        }
    }
}