using Application.Interfaces.Contexts;
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

namespace Application.Services.Wallets.Commands.TransferWallet
{
    public class TransferWalletCommand : IRequest<ResultDto>
    {
        public RequestTransferWalletDto Request { get; set; }
        public TransferWalletCommand(RequestTransferWalletDto request)
        {
            Request = request;
        }
    }
    public class TransferWalletHandler : IRequestHandler<TransferWalletCommand, ResultDto>
    {
        private readonly ILogger<TransferWalletHandler> _logger;
        private readonly IDataBaseContext _context;
        public TransferWalletHandler(IDataBaseContext context,
            ILogger<TransferWalletHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public Task<ResultDto> Handle(TransferWalletCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var fromWallet = _context.Wallets
                    .FirstOrDefault(x => x.AccountNumber == request.Request.FromAccountNumber);

                if (fromWallet == null)
                {
                    return Task.FromResult(new ResultDto
                    {
                        IsSuccess = false,
                        Message = $"کیف پولی مبدا یی با شماره حساب '{request.Request.FromAccountNumber}' یافت نشد"
                    });
                }

                var toWallet = _context.Wallets
                    .FirstOrDefault(x => x.AccountNumber == request.Request.ToAccountNumber);

                if (toWallet == null)
                {
                    return Task.FromResult(new ResultDto
                    {
                        IsSuccess = false,
                        Message = $"کیف پولی مقصدی با شماره حساب '{request.Request.ToAccountNumber}' یافت نشد"
                    });
                }

                if (request.Request.Amount > fromWallet.WithdrawalBalance)
                {
                    return Task.FromResult(new ResultDto
                    {
                        IsSuccess = false,
                        Message = "موجودی حساب کافی نیست"
                    });
                }

                fromWallet.TotalInventory -= request.Request.Amount;
                fromWallet.WithdrawalBalance -= request.Request.Amount;

                toWallet.TotalInventory += request.Request.Amount;
                toWallet.WithdrawalBalance += request.Request.Amount;

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

    public class RequestTransferWalletDto
    {
        /// <summary>
        /// شماره حساب مبدا
        /// </summary>
        [Required]
        [Range(100000, 999999)]
        public int FromAccountNumber { get; set; }

        /// <summary>
        /// شماره حساب مقصد
        /// </summary>
        [Required]
        [Range(100000, 999999)]
        public int ToAccountNumber { get; set; }

        /// <summary>
        /// مبلغ واریزی
        /// </summary>S
        [Required]
        [Range(0, long.MaxValue)]
        public long Amount { get; set; }
    }
    public class RequestTransferWalletDtoValidator : AbstractValidator<RequestTransferWalletDto>
    {
        public RequestTransferWalletDtoValidator()
        {
            RuleFor(p => p.FromAccountNumber)
                .NotNull()
                .NotEmpty()
                .When(p => p.FromAccountNumber >= 100000 && p.FromAccountNumber <= 999999).WithMessage("شماره حساب مقصد باید یک عدد شش رقمی باشد");

            RuleFor(p => p.ToAccountNumber)
                .NotNull()
                .NotEmpty()
                .When(p => p.ToAccountNumber >= 100000 && p.ToAccountNumber <= 999999).WithMessage("شماره حساب مبدا باید یک عدد شش رقمی باشد");

            RuleFor(p => p.Amount)
                .NotNull()
                .NotEmpty()
                .When(p => p.Amount <= decimal.Zero).WithMessage("مبلغ واریزی باید بیش تر از صفر باشد");
        }
    }
}
