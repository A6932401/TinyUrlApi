using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TinyUrlApp.Logic.Interface;
using TinyUrlApp.Model;

namespace TinyUrlApp.EndPoints
{
    public static class LinkEndpoint
    {        
        public static IEndpointRouteBuilder LinkEndpoints(this IEndpointRouteBuilder endpointRoute)
        {
            endpointRoute.MapPost("/link", async (LinkAdd link, IValidator<LinkAdd> validator,IEndPointLogic logic) =>
            {
                var result = await validator.ValidateAsync(link);

                if (!result.IsValid)
                {
                    var errors = result.Errors.FirstOrDefault().ErrorMessage;

                    var responseValue = new ResponceModel<string>
                    {
                        status = "Failed",
                        response = errors,
                        message = errors
                    };

                    return Results.BadRequest(responseValue);
                }

               var value = logic.AddLink(link);
                var successResponse = new ResponceModel<ReturnLink>
                {
                    status = "Success",
                    response = value,
                    message = "Link added successfully"
                };

                return Results.Ok(successResponse);
            });
            endpointRoute.MapGet("/link",async (IEndPointLogic logic) =>
            {
                var data = logic.GetLinkByRule(false);
                if (data.Count > 0)
                {
                    var successResponse = new ResponceModel<List<ReturnLink>>
                    {
                        status = "Success",
                        response = data,
                        message = ""
                    };
                    return Results.Ok(successResponse);
                }
                else
                {
                    return Results.NoContent();
                }
            });
            endpointRoute.MapGet("/link/code", async (int id, IValidator<int> validator, IEndPointLogic logic) =>
            {
                var result = await validator.ValidateAsync(id);
                if (!result.IsValid)
                {
                    var errors = result.Errors.FirstOrDefault().ErrorMessage;

                    var responseValue = new ResponceModel<string>
                    {
                        status = "Failed",
                        response = errors,
                        message = errors
                    };

                    return Results.BadRequest(responseValue);
                }
                var data = logic.GetLinkById( id);
                if (!string.IsNullOrEmpty(data.originallink))
                {
                    var successResponse = new ResponceModel<ReturnLink>
                    {
                        status = "Success",
                        response = data,
                        message = ""
                    };
                    return Results.Ok(successResponse);
                }
                else
                {
                    return Results.NoContent();
                }
            });
            endpointRoute.MapDelete("/link", async (int id, IValidator<int> validator, IEndPointLogic logic) =>
            {
                var result = await validator.ValidateAsync(id);
                if (!result.IsValid)
                {
                    var errors = result.Errors.FirstOrDefault().ErrorMessage;

                    var responseValue = new ResponceModel<string>
                    {
                        status = "Failed",
                        response = errors,
                        message = errors
                    };

                    return Results.BadRequest(responseValue);
                }
                var data = logic.DeleteEndPoint(id);
                if (data.Item1 == true)
                {
                    var successResponse = new ResponceModel<List<string>>
                    {
                        status = "Success",
                    };
                    return Results.Ok(successResponse);
                }
                else
                {
                    var successResponse = new ResponceModel<List<string>>
                    {
                        status = "Failed",
                        message = data.Item2
                    };
                    return Results.Ok(successResponse);
                }
            });
            endpointRoute.MapDelete("/link/All", async (IEndPointLogic logic) =>
            {
               
                var data = logic.DeleteAllEndPoint();
                if (data.Item1 == true)
                {
                    var successResponse = new ResponceModel<List<string>>
                    {
                        status = "Success",
                    };
                    return Results.Ok(successResponse);
                }
                else
                {
                    var successResponse = new ResponceModel<List<string>>
                    {
                        status = "Failed",
                        message = data.Item2
                    };
                    return Results.Ok(successResponse);
                }
            });
            endpointRoute.MapPut("/link", async (int id, IValidator<int> validator, IEndPointLogic logic) =>
            {
                var result = await validator.ValidateAsync(id);
                if (!result.IsValid)
                {
                    var errors = result.Errors.FirstOrDefault().ErrorMessage;

                    var responseValue = new ResponceModel<string>
                    {
                        status = "Failed",
                        response = errors,
                        message = errors
                    };

                    return Results.BadRequest(responseValue);
                }
                var data = logic.UpdateClickCount(id);
                if (data.Item1 == true)
                {
                    var successResponse = new ResponceModel<string>
                    {
                        status = "Success",
                        message = "Updated Successfully",
                        response = data.Item2
                    };
                    return Results.Ok(successResponse);
                    //return Results.Redirect(data.Item2, permanent: true);
                }
                else
                {
                    var successResponse = new ResponceModel<string>
                    {
                        status = "Failed",
                        message = data.Item2
                    };
                    return Results.Ok(successResponse);
                }
            });

            return endpointRoute;
        }
    }
}
