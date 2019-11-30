using MediatR;
using VoteMonitor.Api.Form.Models;

namespace VoteMonitor.Api.Form.Queries
{
	public class AddOptionCommand : IRequest<OptionDto>
	{
		public OptionDto Option { get; }

		public AddOptionCommand(OptionDto option)
		{
			Option = option;
		}
	}
}