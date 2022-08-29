using Application.Interfaces.Contexts;
using Common.Dto;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Wallets.Commands.DeleteWallet
{
    public class DeleteWalletCommand : IRequest<ResultDto>
    {
        public int Id { get; set; }
        public DeleteWalletCommand(int id)
        {
            Id = id;
        }
    }

    public class DeleteWalletHandler : IRequestHandler<DeleteWalletCommand, ResultDto>
    {
        private readonly ILogger<DeleteWalletHandler> _logger;
        private readonly IDataBaseContext _context;
        public DeleteWalletHandler(IDataBaseContext context,
            ILogger<DeleteWalletHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public Task<ResultDto> Handle(DeleteWalletCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var wallet = _context.Wallets
                    .FirstOrDefault(x => x.Id == request.Id);

                if (wallet == null)
                {
                    return Task.FromResult(new ResultDto
                    {
                        IsSuccess = false,
                        Message = $"کیف پولی با شناسه '{request.Id}' یافت نشد"
                    });
                }

                _context.Wallets.Remove(wallet);
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
}
