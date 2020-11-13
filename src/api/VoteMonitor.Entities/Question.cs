﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMonitor.Entities
{
    public partial class Question : IHierarchicalEntity<OptionToQuestion>, IIdentifiableEntity
    {
        public Question()
        {
            Notes = new HashSet<Note>();
            OptionsToQuestions = new HashSet<OptionToQuestion>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Code { get; set; }
        public int IdSection { get; set; }
        public QuestionType QuestionType { get; set; }
        [Required, MaxLength(1000)]
        public string Text { get; set; }
        public string Hint { get; set; }
        public int OrderNumber { get; set; }

        public virtual ICollection<Note> Notes { get; set; }
        public virtual ICollection<OptionToQuestion> OptionsToQuestions { get; set; }
        public virtual FormSection FormSection { get; set; }

        ICollection<OptionToQuestion> IHierarchicalEntity<OptionToQuestion>.Children { get => OptionsToQuestions; set => OptionsToQuestions = value; }
    }
}
