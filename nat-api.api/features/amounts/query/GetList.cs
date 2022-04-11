using System;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using nat_api.core.results;
using nat_api.data.context;
using System.Threading;
using System.Threading.Tasks;
using nat_api.data.models.amount;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace nat_api.api.features.amounts.query
{
    public class GetList
    {
        public class Query : IRequest<Result<List<Response>>>
        {
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {}
        }

        public class Response : AmountResponse
        {}

        public class Handler : IRequestHandler<Query, Result<List<Response>>>
        {
            private readonly DataContext repository;
            private readonly ILogger<Handler> logger;
            private readonly AutoMapper.IConfigurationProvider configuration;

            public Handler(
                DataContext repository,
                ILogger<Handler> logger,
                AutoMapper.IConfigurationProvider configuration
            )
            {
                this.repository = repository;
                this.logger = logger;
                this.configuration = configuration;
            }

            public async Task<Result<List<Response>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (this.logger.BeginScope("Amount - Get List"))
                {
                    this.logger.LogInformation("Begin");
                    var response = await this.repository.Amounts
                        .ProjectTo<Response>(this.configuration)
                        .ToListAsync();

                    this.logger.LogInformation($"Response - {JsonConvert.SerializeObject(response)}");
                    return Result<List<Response>>.Success(response);
                }
            }

            public class MappingProfile : Profile
            {
                public MappingProfile()
                {
                    CreateMap<Amount, Response>();
                }
            }
        }
    }
}