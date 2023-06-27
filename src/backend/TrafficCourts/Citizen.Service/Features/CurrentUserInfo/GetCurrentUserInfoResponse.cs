using TrafficCourts.Citizen.Service.Models.OAuth;

namespace TrafficCourts.Citizen.Service.Features.CurrentUserInfo;

public class GetCurrentUserInfoResponse
{
    public static readonly GetCurrentUserInfoResponse Empty = new GetCurrentUserInfoResponse();

    private GetCurrentUserInfoResponse()
    {
    }

    public GetCurrentUserInfoResponse(UserInfo userInfo)
    {
        UserInfo = userInfo ?? throw new ArgumentNullException(nameof(userInfo));
    }

    public UserInfo? UserInfo { get; }
}
