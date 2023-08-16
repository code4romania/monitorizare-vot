using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Models
{
    public class FormResponseModel
    {
        public int Id { get; set; }
        public string Code { get; set; }

        public string Description { get; set; }

        public int CurrentVersion { get; set; }

        public bool Diaspora { get; set; }

        public IReadOnlyList<FormSectionResponseModel> FormSections { get; set; }
    }

    public class FormSectionResponseModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public IReadOnlyList<QuestionResponseModel> Questions { get; set; }
    }
    
    public class QuestionResponseModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public QuestionType QuestionType { get; set; }
        public string Text { get; set; }
        public string Hint { get; set; }

        public IReadOnlyList<OptionsResponseModel> Options { get; set; }
        public int QuestionId { get; set; }
    }

    public class OptionsResponseModel
    {
        /// <summary>
        /// Option to question Id. This id is used to link questions with existing options.
        /// </summary>
        public int Id { get; set; }

        public int OptionId { get; set; }

        public bool IsFreeText { get; set; }
        public string Text { get; set; }
        public string Hint { get; set; }
        public bool IsFlagged { get; set; }
    }
}
