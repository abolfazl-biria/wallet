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

namespace Application.Services.Wallets.Commands.DepositWallet
{
    public class DepositWalletCommand : IRequest<ResultDto>
    {
        public RequestDepositWalletDto Request { get; set; }
        public DepositWalletCommand(RequestDepositWalletDto request)
        {
            Request = request;
        }
    }
    public class DepositWalletHandler : IRequestHandler<DepositWalletCommand, ResultDto>
    {
        private readonly ILogger<DepositWalletHandler> _logger;
        private readonly IDataBaseContext _context;
        public DepositWalletHandler(IDataBaseContext context,
            ILogger<DepositWalletHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public Task<ResultDto> Handle(DepositWalletCommand request, CancellationToken cancellationToken)
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

                wallet.TotalInventory += request.Request.Amount;
                wallet.WithdrawalBalance += request.Request.Amount;

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

    public class RequestDepositWalletDto
    {
        /// <summary>
        /// شماره حساب
        /// </summary>
        [Required]
        [Range(100000, 999999)]
        public int AccountNumber { get; set; }

        /// <summary>
        /// مبلغ واریزی
        /// </summary>S
        [Required]
        [Range(0, long.MaxValue)]
        public long Amount { get; set; }
    }
    public class RequestDepositWalletDtoValidator : AbstractValidator<RequestDepositWalletDto>
    {
        public RequestDepositWalletDtoValidator()
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
