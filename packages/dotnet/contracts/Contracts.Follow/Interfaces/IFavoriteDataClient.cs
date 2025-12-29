using Waggle.Common.Results.Core;
using Waggle.Contracts.Follow.Grpc;

namespace Waggle.Contracts.Follow.Interfaces
{
    public interface IFollowDataClient
    {
        Task<Result<GetFollowsResponse>> GetFollowsAsync(GetFollowsRequest request);
        Task<Result<GetFollowByIdResponse>> GetFollowByIdAsync(GetFollowByIdRequest request);
        Task<Result<GetFollowersResponse>> GetFollowersAsync(GetFollowersRequest request);
        Task<Result<GetFollowingResponse>> GetFollowingAsync(GetFollowingRequest request);
        Task<Result<GetFollowerCountResponse>> GetFollowerCountAsync(GetFollowerCountRequest request);
        Task<Result<GetFollowingCountResponse>> GetFollowingCountAsync(GetFollowingCountRequest request);
        Task<Result<IsFollowingResponse>> IsFollowingAsync(IsFollowingRequest request);
        Task<Result<CreateFollowResponse>> CreateFollowAsync(CreateFollowRequest request);
        Task<Result> DeleteFollowAsync(DeleteFollowRequest request);
    }
}