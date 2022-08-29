using Application.Interfaces.Contexts;
using Common.Dto;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Application.Services.Wallets.Queries.GetWalletInventory
{
    public class GetWalletInventoryRequest: IRequest<ResultDto<long>>
    {
        public int AccountNumber { get; set; }
        public GetWalletInventoryRequest(int accountNumber)
        {
            AccountNumber = accountNumber;
        }
    }
    public class GetWalletInventoryHandler : IRequestHandler<GetWalletInventoryRequest, ResultDto<long>>
    {
        private readonly ILogger<GetWalletInventoryHandler> _logger;
        private readonly IDataBaseContext _context;
        public GetWalletInventoryHandler(IDataBaseContext context,
            ILogger<GetWalletInventoryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public Task<ResultDto<long>> Handle(GetWalletInventoryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var wallet = _context.Wallets
                   .FirstOrDefault(x => x.AccountNumber == request.AccountNumber);

                if (wallet == null)
                {
                    return Task.FromResult(new ResultDto<long>
                    {
                        Data = 0,
                        IsSuccess = false,
                        Message = $"کیف پولی با شماره حساب '{request.AccountNumber}' یافت نشد"
                    });
                }

                return Task.FromResult(new ResultDto<long>
                {
                    Data = wallet.WithdrawalBalance,
                    IsSuccess = true,
                    Message = "با موفقیت انجام شد"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stopped program because of exception");

                return Task.FromResult(new ResultDto<long>
                {
                    Data = 0,
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }
    }
}
