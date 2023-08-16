using System;
using System.Collections.Generic;

namespace VoteMonitor.Api.Form.Models
{
    public class FilledFormResponseModel
    {
        public int FormId { get; set; }
        public int ObserverId { get; set; }
        public IReadOnlyList<AnswerResponseModel> FilledInQuestions { get; set; }
        /// <summary>
        /// Last date when an answer from a form was modified.
        /// </summary>
        public DateTime LastModified { get; set; }

        public int PollingStationId { get; set; }
    }

    public class AnswerResponseModel
    {
        public int QuestionId { get; set; }
        public int FormSectionId { get; set; }
        public IReadOnlyList<SelectedOptionResponseModel> SelectedOptions { get; set; }
    }

    public class SelectedOptionResponseModel
    {
        public int Id { get; set; }
        public int OptionId { get; set; }
        public string FreeTextValue { get; set; }
        public DateTime LasModified { get; set; }
    }
}
