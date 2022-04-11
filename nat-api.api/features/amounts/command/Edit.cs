using System;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using nat_api.core.results;
using nat_api.data.context;
using System.Threading;
using System.Threading.Tasks;
using nat_api.data.models.amount;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace nat_api.api.features.amounts.command
{
    public class Edit
    {
        public class Command : IRequest<Result<Response>>
        {
            public long Id { get; set; }
            public decimal Amount {get;set;}
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(q => q.Id).NotEmpty().GreaterThan(0);
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
                using (this.logger.BeginScope("Amount - Edit"))
                {
                    var entityToEdit = await this.repository.Amounts.FirstAsync(i => i.Id == request.Id);
                    this.logger.LogInformation($"Pre-edit - {JsonConvert.SerializeObject(entityToEdit)}");
                    this.mapper.Map(request, entityToEdit);
                    entityToEdit.CalculateAmounts();
                    this.logger.LogInformation($"Post-edit - {JsonConvert.SerializeObject(entityToEdit)}");
                    await this.repository.SaveChangesAsync();

                    var entity = await this.repository.Amounts.FirstAsync(i => i.Id == entityToEdit.Id);

                    return Result<Response>.Success(this.mapper.Map<Response>(entity));
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