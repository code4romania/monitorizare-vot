﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Api.Models
{
    public class FormModel
    {
        public class VersionQuery : IRequest<Dictionary<string,int>>
        {
        }

        public class QuestionsQuery : IRequest<IEnumerable<SectionModel>>
        {
            public string FormCode { get; set; }
            public int CacheHours { get; set; }
            public int CacheMinutes { get; set; }
            public int CacheSeconds { get; set; }
        }
    }
}
