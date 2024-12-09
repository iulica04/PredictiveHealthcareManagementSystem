using Application.DTOs;
using Application.Queries.PrescriptionQueries;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.QueryHandlers.PrescriptionQueryHandlers
{
    public class GetPrescriptionByIdQueryHandler : IRequestHandler<GetPrescriptionByIdQuery, Result<PrescriptionDto>>
    {
        private readonly IPrescriptionRepository repository;
        private readonly IMapper mapper;
        public GetPrescriptionByIdQueryHandler(IPrescriptionRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<Result<PrescriptionDto>> Handle(GetPrescriptionByIdQuery request, CancellationToken cancellationToken)
        {
            var prescription = await repository.GetByIdAsync(request.Id);
            if (prescription == null)
            {
                return Result<PrescriptionDto>.Failure("Prescription not found");
            }
            return Result<PrescriptionDto>.Success(mapper.Map<PrescriptionDto>(prescription));
        }
    }
}
