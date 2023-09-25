using System.ComponentModel.DataAnnotations;

namespace VoteMonitor.Api.Statistics.Models;

public class OptionsFilterModel
{
    [Required]
    public int QuestionId { get; set; }
}