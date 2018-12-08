using System;

namespace VotingIrregularities.Domain.Models
{
    public partial class Rating
    {
        public int RatingId { get; set; }
        public string AttachedFilePath { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public int? QuestionId { get; set; }
        public int ObserverId { get; set; }
        public int VotingSectionId { get; set; }
        public string RatingText { get; set; }

        public virtual Question QuestionNavigationId { get; set; }
        public virtual Observer ObserverNavigationId { get; set; }
        public virtual VotingSection VotingSectionNavigationId { get; set; }
    }
}
