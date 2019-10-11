using MediatR;
using VotingIrregularities.Api.Models.AccountViewModels;

namespace VotingIrregularities.Api.Queries
{
	public class GetNgoAdminUserInfoQuery : IRequest<NgoUserInfo>
	{
		public string UserName { get; }
		public string Password { get; }

		public GetNgoAdminUserInfoQuery(string userName, string password)
		{
			UserName = userName;
			Password = password;
		}
	}
}