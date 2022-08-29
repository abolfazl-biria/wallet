﻿using Application.Interfaces.Contexts;
using Common.Dto;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Wallets.Commands.WithdrawWallet
{
    public class WithdrawWalletCommand : IRequest<ResultDto>
    {
        public RequestWithdrawWalletDto Request { get; set; }
        public WithdrawWalletCommand(RequestWithdrawWalletDto request)
        {
            Request = request;
        }
    }
    public class WithdrawWalletHandler : IRequestHandler<WithdrawWalletCommand, ResultDto>
    {
        private readonly ILogger<WithdrawWalletHandler> _logger;
        private readonly IDataBaseContext _context;
        public WithdrawWalletHandler(IDataBaseContext context,
            ILogger<WithdrawWalletHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public Task<ResultDto> Handle(WithdrawWalletCommand request, CancellationToken cancellationToken)
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

                if (request.Request.Amount > wallet.WithdrawalBalance)
                {
                    return Task.FromResult(new ResultDto
                    {
                        IsSuccess = false,
                        Message = "موجودی حساب کافی نیست"
                    });
                }

                wallet.TotalInventory -= request.Request.Amount;
                wallet.WithdrawalBalance -= request.Request.Amount;

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

    public class RequestWithdrawWalletDto
    {
        /// <summary>
        /// شماره حساب
        /// </summary>
        [Required]
        [Range(100000, 999999)]
        public int AccountNumber { get; set; }

        /// <summary>
        /// مبلغ برداشتی
        /// </summary>S
        [Required]
        [Range(0, long.MaxValue)]
        public long Amount { get; set; }
    }
    public class RequestWithdrawWalletDtoValidator : AbstractValidator<RequestWithdrawWalletDto>
    {
        public RequestWithdrawWalletDtoValidator()
        {
            RuleFor(p => p.AccountNumber)
                .NotNull()
                .NotEmpty()
                .When(p => p.AccountNumber >= 100000 && p.AccountNumber <= 999999).WithMessage("شماره حساب باید یک عدد شش رقمی باشد");

            RuleFor(p => p.Amount)
                .NotNull()
                .NotEmpty()
                .When(p => p.Amount <= decimal.Zero).WithMessage("مبلغ واریزی باید بیش تر از صفر باشد");
        }
    }
}
