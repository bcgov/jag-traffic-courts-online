using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisputeApi.Web.Features.Disputes.Queries
{
    public class GetAllDisputesQuery : IRequest<IEnumerable<GetDisputeResponse>>
    {
    }
}
