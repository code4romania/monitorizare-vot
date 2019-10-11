using MediatR;
using VotingIrregularities.Api.Models.AccountViewModels;

namespace VotingIrregularities.Api.Queries
{
	public class GetObserverUserInfoQuery: IRequest<RegisteredObserverInfo>
	{
		public string UserName { get; }
		public string Password { get; }
		public string UDID { get; }

		public GetObserverUserInfoQuery(string userName, string password, string udid)
		{
			UserName = userName;
			Password = password;
			UDID = udid;
		}
	}
}