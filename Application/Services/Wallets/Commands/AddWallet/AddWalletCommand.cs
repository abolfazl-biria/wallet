using Application.Interfaces.Contexts;
using Common.Dto;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Wallets.Commands.AddWallet
{
    public class AddWalletCommand : IRequest<ResultDto>
    {
        public RequestAddWalletDto Request { get; set; }
        public AddWalletCommand(RequestAddWalletDto request)
        {
            Request = request;
        }
    }
    public class AddWalletHandler : IRequestHandler<AddWalletCommand, ResultDto>
    {
        private readonly ILogger<AddWalletHandler> _logger;
        private readonly IDataBaseContext _context;
        public AddWalletHandler(ILogger<AddWalletHandler> logger,
            IDataBaseContext context)
        {
            _logger = logger;
            _context = context;
        }
        public Task<ResultDto> Handle(AddWalletCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var AccountNumber_is_unique =
                    _context.Wallets
                    .FirstOrDefault(x => x.AccountNumber == request.Request.AccountNumber) == null;

                if (!AccountNumber_is_unique)
                {
                    return Task.FromResult(new ResultDto
                    {
                        IsSuccess = false,
                        Message = "شماره حساب از قبل موجود می باشد"
                    });
                }

                var wallet = new Wallet
                {
                    AccountNumber = request.Request.AccountNumber,
                    BlockedInventory = request.Request.BlockedInventory,
                    TotalInventory = request.Request.TotalInventory,
                    UserName = request.Request.UserName,
                    WithdrawalBalance = request.Request.TotalInventory - request.Request.BlockedInventory,
                };

                _context.Wallets.Add(wallet);
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

    public class RequestAddWalletDto
    {
        /// <summary>
        /// شماره حساب
        /// </summary>
        [Required]
        [Range(100000, 999999)]
        public int AccountNumber { get; set; }

        /// <summary>
        /// نام و نام خانوادگی صاحب حساب
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// موجودی بلاک شده
        /// </summary>
        [Required]
        [Range(0, long.MaxValue)]
        public long BlockedInventory { get; set; }

        /// <summary>
        /// موجودی کل
        /// </summary>S
        [Required]
        [Range(0, long.MaxValue)]
        public long TotalInventory { get; set; }
    }
    public class RequestAddWalletDtoValidator : AbstractValidator<RequestAddWalletDto>
    {
        public RequestAddWalletDtoValidator()
        {
            RuleFor(p => p.UserName).NotNull().NotEmpty();

            RuleFor(p => p.TotalInventory)
                .NotNull()
                .NotEmpty()
                .When(p => p.TotalInventory < decimal.Zero).WithMessage("موجودی کل نباید کم تر از صفر باشد");

            RuleFor(p => p.BlockedInventory)
                .NotNull()
                .NotEmpty()
                .When(p => p.BlockedInventory < decimal.Zero).WithMessage("موجودی بلاک شده نباید کم تر از صفر باشد")
                .When(p => p.BlockedInventory > p.TotalInventory).WithMessage("موجودی بلاک شده نباید بیشتر تر از موجودی کل باشد");

            RuleFor(p => p.AccountNumber)
                .NotNull()
                .NotEmpty()
                .When(p => p.AccountNumber >= 100000 && p.AccountNumber <= 999999).WithMessage("شماره حساب باید یک عدد شش رقمی باشد");
        }
    }
}
