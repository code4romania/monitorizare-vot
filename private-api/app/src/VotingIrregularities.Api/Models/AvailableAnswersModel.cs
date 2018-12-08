using System;
using System.Collections.Generic;

namespace VotingIrregularities.Api.Models
{
    public class AvailableAnswersModel
    {
        public int IdOptiune { get; set; }
        public string TextOptiune { get; set; }
        public bool SeIntroduceText { get; set; }

    }
}
