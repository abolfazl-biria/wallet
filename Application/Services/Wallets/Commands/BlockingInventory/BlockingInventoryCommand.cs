using Application.Interfaces.Contexts;
using Common.Dto;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Wallets.Commands.BlockingInventory
{
    public class BlockingInventoryCommand : IRequest<ResultDto>
    {
        public RequestBlockingInventoryDto Request { get; set; }
        public BlockingInventoryCommand(RequestBlockingInventoryDto request)
        {
            Request = request;
        }
    }
    public class BlockingInventoryHandler : IRequestHandler<BlockingInventoryCommand, ResultDto>
    {
        private readonly IDataBaseContext _context;
        private readonly ILogger<BlockingInventoryHandler> _logger;
        public BlockingInventoryHandler(IDataBaseContext context,
            ILogger<BlockingInventoryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public Task<ResultDto> Handle(BlockingInventoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var wallet = _context.Wallets
                    .FirstOrDefault(x => x.AccountNumber == request.Request.AccountNumber);

                if (wallet == null)
                {
                    return Task.FromResult(new ResultDto
                    {
                        IsSuccess = false,
                        Message = $"کیف پولی با شماره حساب '{request.Request.AccountNumber}' یافت نشد"
                    });
                }

                if (request.Request.BlockedInventory > wallet.TotalInventory)
                {
                    return Task.FromResult(new ResultDto
                    {
                        IsSuccess = false,
                        Message = $"حداکثر مبلغ قابل بلاک '{wallet.TotalInventory}' می باشد"
                    });
                }

                wallet.BlockedInventory = request.Request.BlockedInventory;
                wallet.WithdrawalBalance = wallet.TotalInventory - request.Request.BlockedInventory;

                _context.SaveChanges();

                return Task.FromResult(new ResultDto
                {
                    IsSuccess = true,
                    Message = "با موفقیت انجام شد"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stopped program because of exception");

                return Task.FromResult(new ResultDto
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }
    }

    public class RequestBlockingInventoryDto
    {
        /// <summary>
        /// شماره حساب
        /// </summary>
        [Required]
        [Range(100000, 999999)]
        public int AccountNumber { get; set; }

        /// <summary>
        /// موجودی بلاک شده
        /// </summary>
        [Required]
        [Range(0, long.MaxValue)]
        public long BlockedInventory { get; set; }
    }
    public class RequestBlockingInventoryDtoValidator : AbstractValidator<RequestBlockingInventoryDto>
    {
        public RequestBlockingInventoryDtoValidator()
        {
            RuleFor(p => p.BlockedInventory)
                .NotNull()
                .NotEmpty()
                .When(p => p.BlockedInventory < decimal.Zero).WithMessage("موجودی بلاک شده نباید کم تر از صفر باشد");

            RuleFor(p => p.AccountNumber)
                .NotNull()
                .NotEmpty()
                .When(p => p.AccountNumber >= 100000 && p.AccountNumber <= 999999).WithMessage("شماره حساب باید یک عدد شش رقمی باشد");
        }
    }
}
