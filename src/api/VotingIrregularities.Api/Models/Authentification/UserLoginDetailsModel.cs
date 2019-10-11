using System.ComponentModel.DataAnnotations;

namespace VotingIrregularities.Api.Models.Authentification
{
	public class UserLoginDetailsModel
	{
		/// <summary>
		/// Username or phone number
		/// </summary>
		[Required]
		public string Username { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		/// <summary>
		/// This is the unique identifier of the mobile device
		/// </summary>
		public string UDID { get; set; }
	}
}