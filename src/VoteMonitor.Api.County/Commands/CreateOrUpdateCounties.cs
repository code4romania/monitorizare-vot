﻿using MediatR;
using Microsoft.AspNetCore.Http;

namespace VoteMonitor.Api.County.Commands
{
    public class CreateOrUpdateCounties: IRequest<object>
    {
        public IFormFile File { get; }

        public CreateOrUpdateCounties(IFormFile file)
        {
            File = file;
        }
    }
}