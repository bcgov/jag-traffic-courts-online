using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisputeApi.Web.Features.Disputes.Commands
{
    public class CreateDisputeResponse
    {
        public int Id { get; set; }

        public void Mapping(Profile profile)
        {
            //profile.CreateMap<CreateDisputeResponse, CreateDisputeDto>();
        }
    }
}
