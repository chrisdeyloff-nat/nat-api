using AutoMapper;
using FluentValidation;
using MediatR;
using nat_api.core.results;
using nat_api.data.context;
using System.Threading;
using System.Threading.Tasks;
using nat_api.data.models.amount;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace nat_api.api.features.amounts.command
{
    public class Create
    {
        public class Command : IRequest<Result<Response>>
        {
            public decimal Amount {get;set;}
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(q => q.Amount).NotEmpty().GreaterThanOrEqualTo(0).LessThanOrEqualTo(Int32.MaxValue);
            }
        }

        public class Response : AmountResponse
        {}

        public class Handler : IRequestHandler<Command, Result<Response>>
        {
            private readonly DataContext repository;
            private readonly ILogger<Handler> logger;
            private readonly IMapper mapper;

            public Handler(
                DataContext repository,
                ILogger<Handler> logger,
                IMapper mapper
            )
            {
                this.repository = repository;
                this.logger = logger;
                this.mapper = mapper;
            }

            public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (this.logger.BeginScope("Amount - Create"))
                {
                    var newEntity = this.mapper.Map<Amount>(request);
                    newEntity.CalculateAmounts();
                    this.logger.LogInformation($"Calculated Amount - {JsonConvert.SerializeObject(newEntity)}");
                    this.repository.Amounts.Add(newEntity);
                    await this.repository.SaveChangesAsync();
                    this.logger.LogInformation($"Saved - {JsonConvert.SerializeObject(newEntity)}");

                    return Result<Response>.Success(this.mapper.Map<Response>(newEntity));
                }
            }

            public class MappingProfile : Profile
            {
                public MappingProfile()
                {
                    CreateMap<Command, Amount>()
                        .ForMember(d => d.Value, opt => opt.MapFrom(i => i.Amount));
                    CreateMap<Amount, Response>();
                }
            }
        }
    }
}