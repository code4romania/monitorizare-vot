namespace VotingIrregularities.Api.Models.AccountViewModels
{
	public class RegisteredObserverInfo
	{
		public bool IsAuthenticated { get; set; }

		public int ObserverId { get; set; }

		public bool FirstAuthentication { get; set; }
		public int IdNgo { get; set; }
		public string UDID { get; set; }
		public string Phone { get; set; }

	}
}
