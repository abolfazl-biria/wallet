using Application.Interfaces.Contexts;
using AutoMapper;
using Common.Dto;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Wallets.Queries.GetAllWallets
{
    public class GetAllWalletsRequest : IRequest<ResultDto<List<ResultGetAllWalletsDto>>>
    {
    }
    public class GetAllWalletsHandler : IRequestHandler<GetAllWalletsRequest, ResultDto<List<ResultGetAllWalletsDto>>>
    {
        private readonly ILogger<GetAllWalletsHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IDataBaseContext _context;
        public GetAllWalletsHandler(IDataBaseContext context,
            IMapper mapper,
            ILogger<GetAllWalletsHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }
        public Task<ResultDto<List<ResultGetAllWalletsDto>>> Handle(GetAllWalletsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var wallets = _context.Wallets
                    .AsQueryable();

                var result = _mapper.ProjectTo<ResultGetAllWalletsDto>(wallets).ToList();

                return Task.FromResult(new ResultDto<List<ResultGetAllWalletsDto>>
                {
                    Data = result,
                    IsSuccess = true,
                    Message = "با موفقیت انجام شد"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stopped program because of exception");

                return Task.FromResult(new ResultDto<List<ResultGetAllWalletsDto>>
                {
                    Data = new List<ResultGetAllWalletsDto> { },
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }
    }

    public class ResultGetAllWalletsDto
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
