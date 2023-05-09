using MediatR;

namespace TrafficCourts.Citizen.Service.Features.CurrentUserInfo;

public class GetCurrentUserInfoRequest : IRequest<GetCurrentUserInfoResponse>
{
    /// <summary>
    /// The default request.
    /// </summary>
    public static readonly GetCurrentUserInfoRequest Default = new GetCurrentUserInfoRequest();

    private GetCurrentUserInfoRequest()
    {
    }
}
