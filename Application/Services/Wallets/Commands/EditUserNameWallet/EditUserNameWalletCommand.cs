using Application.Interfaces.Contexts;
using Application.Services.Wallets.Commands.TransferWallet;
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

namespace Application.Services.Wallets.Commands.EditUserNameWallet
{
    public class EditUserNameWalletCommand : IRequest<ResultDto>
    {
        public RequestEditUserNameWalletDto Request { get; set; }
        public EditUserNameWalletCommand(RequestEditUserNameWalletDto request)
        {
            Request = request;
        }
    }
    public class EditUserNameWalletHandler : IRequestHandler<EditUserNameWalletCommand, ResultDto>
    {
        private readonly ILogger<EditUserNameWalletHandler> _logger;
        private readonly IDataBaseContext _context;
        public EditUserNameWalletHandler(IDataBaseContext context,
            ILogger<EditUserNameWalletHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public Task<ResultDto> Handle(EditUserNameWalletCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var wallet = _context.Wallets
                    .FirstOrDefault(x => x.Id == request.Request.Id);

                if (wallet == null)
                {
                    return Task.FromResult(new ResultDto
                    {
                        IsSuccess = false,
                        Message = $"کیف پولی با شناسه '{request.Request.Id}' یافت نشد"
                    });
                }

                wallet.UserName = request.Request.UserName;
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

    public class RequestEditUserNameWalletDto
    {
        public int Id { get; set; }

        /// <summary>
        /// نام و نام خانوادگی صاحب حساب
        /// </summary>
        [Required]
        public string UserName { get; set; }
    }
    public class RequestEditUserNameWalletDtoValidator : AbstractValidator<RequestEditUserNameWalletDto>
    {
        public RequestEditUserNameWalletDtoValidator()
        {
            RuleFor(p => p.UserName).NotNull()
                .NotEmpty();
        }
    }
}
