using Application.Interfaces.Contexts;
using AutoMapper;
using Common.Dto;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Wallets.Queries.GetWalletById
{
    public class GetWalletByIdRequest : IRequest<ResultDto<ResultGetWalletByIdDto>>
    {
        public int Id { get; set; }
        public GetWalletByIdRequest(int id)
        {
            Id = id;
        }
    }
    public class GetWalletByIdHandler : IRequestHandler<GetWalletByIdRequest, ResultDto<ResultGetWalletByIdDto>>
    {
        private readonly ILogger<GetWalletByIdHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IDataBaseContext _context;
        public GetWalletByIdHandler(IDataBaseContext context,
            IMapper mapper,
            ILogger<GetWalletByIdHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }
        public Task<ResultDto<ResultGetWalletByIdDto>> Handle(GetWalletByIdRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var wallet = _context.Wallets
                   .FirstOrDefault(x => x.Id == request.Id);

                if (wallet == null)
                {
                    return Task.FromResult(new ResultDto<ResultGetWalletByIdDto>
                    {
                        IsSuccess = false,
                        Data = new ResultGetWalletByIdDto { },
                        Message = $"کیف پولی با شناسه '{request.Id}' یافت نشد"
                    });
                }

                var result = _mapper.Map<ResultGetWalletByIdDto>(wallet);

                return Task.FromResult(new ResultDto<ResultGetWalletByIdDto>
                {
                    Data = result,
                    IsSuccess = true,
                    Message = "با موفقیت انجام شد"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stopped program because of exception");

                return Task.FromResult(new ResultDto<ResultGetWalletByIdDto>
                {
                    Data = new ResultGetWalletByIdDto { },
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }
    }

    public class ResultGetWalletByIdDto
    {
        /// <summary>
        /// شناسه
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// شماره حساب
        /// </summary>
        public int AccountNumber { get; set; }

        /// <summary>
        /// نام و نام خانوادگی صاحب حساب
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// موجودی قابل برداشت
        /// </summary>
        public long WithdrawalBalance { get; set; }

        /// <summary>
        /// موجودی بلاک شده
        /// </summary>
        public long BlockedInventory { get; set; }

        /// <summary>
        /// موجودی کل
        /// </summary>S
        public long TotalInventory { get; set; }
    }
}
