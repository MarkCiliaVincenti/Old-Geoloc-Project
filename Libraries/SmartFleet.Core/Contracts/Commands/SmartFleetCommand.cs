using System;
using FluentValidation.Results;
using MediatR;

namespace SmartFleet.Core.Contracts.Commands
{
    public class SmartFleetCommand :IRequest<ValidationResult>
    {
        public Guid CmdId { get; set; }
        public SmartFleetCommand()
        {
            CmdId = Guid.NewGuid();
        }

    }
}
