using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Persistance.Contexts;
using MediatR;
using Application.Services.Wallets.Queries.GetAllWallets;
using Common.Dto;
using Application.Services.Wallets.Queries.GetWalletById;
using Application.Services.Wallets.Commands.EditUserNameWallet;
using Application.Services.Wallets.Commands.AddWallet;
using Application.Services.Wallets.Commands.BlockingInventory;
using Application.Services.Wallets.Commands.DeleteWallet;
using Application.Services.Wallets.Queries.GetWalletInventory;
using Application.Services.Wallets.Commands.DepositWallet;
using Application.Services.Wallets.Commands.TransferWallet;
using Application.Services.Wallets.Commands.WithdrawWallet;
using Microsoft.AspNetCore.Authorization;

namespace EndPoint.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class WalletsController : ControllerBase
    {
        private readonly ILogger<WalletsController> logger;
        private readonly IMediator _mediator;
        public WalletsController(ILogger<WalletsController> logger,
            IMediator mediator)
        {
            this.logger = logger;
            _mediator = mediator;
        }

        [Route("api/Wallets/GetWallets")]
        [HttpGet]
        public ResultDto<List<ResultGetAllWalletsDto>> GetWallets()
        {
            try
            {
                return _mediator.Send(new GetAllWalletsRequest()).Result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Stopped program because of exception");
                return new ResultDto<List<ResultGetAllWalletsDto>>
                {
                    IsSuccess = false,
                    Data = new List<ResultGetAllWalletsDto> { },
                    Message = ex.Message
                };
            }
        }

        [Route("api/Wallets/GetWallet")]
        [HttpGet]
        public ResultDto<ResultGetWalletByIdDto> GetWallet(int id)
        {
            try
            {
                return _mediator.Send(new GetWalletByIdRequest(id)).Result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Stopped program because of exception");
                return new ResultDto<ResultGetWalletByIdDto>
                {
                    IsSuccess = false,
                    Data = new ResultGetWalletByIdDto { },
                    Message = ex.Message
                };
            }
        }

        [Route("api/Wallets/GetWalletInventory")]
        [HttpGet]
        public ResultDto<long> GetWalletInventory(int AccountNumber)
        {
            try
            {
                return _mediator.Send(new GetWalletInventoryRequest(AccountNumber)).Result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Stopped program because of exception");
                return new ResultDto<long>
                {
                    IsSuccess = false,
                    Data = 0,
                    Message = ex.Message
                };
            }
        }

        [Route("api/Wallets/AddWallet")]
        [HttpPost]
        public ActionResult<ResultDto> AddWallet(RequestAddWalletDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }

                return _mediator.Send(new AddWalletCommand(request)).Result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Stopped program because of exception");
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        [Route("api/Wallets/BlockingInventory")]
        [HttpPost]
        public ActionResult<ResultDto> BlockingInventory(RequestBlockingInventoryDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }

                return _mediator.Send(new BlockingInventoryCommand(request)).Result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Stopped program because of exception");
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        [Route("api/Wallets/DeleteWallet")]
        [HttpPost]
        public ActionResult<ResultDto> DeleteWallet(int id)
        {
            try
            {
                return _mediator.Send(new DeleteWalletCommand(id)).Result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Stopped program because of exception");
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        [Route("api/Wallets/DepositWallet")]
        [HttpPost]
        public ActionResult<ResultDto> DepositWallet(RequestDepositWalletDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }

                return _mediator.Send(new DepositWalletCommand(request)).Result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Stopped program because of exception");
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        [Route("api/Wallets/EditUserNameWallet")]
        [HttpPost]
        public ActionResult<ResultDto> EditUserNameWallet(RequestEditUserNameWalletDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }

                return _mediator.Send(new EditUserNameWalletCommand(request)).Result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Stopped program because of exception");
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        [Route("api/Wallets/TransferWallet")]
        [HttpPost]
        public ActionResult<ResultDto> TransferWallet(RequestTransferWalletDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }

                return _mediator.Send(new TransferWalletCommand(request)).Result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Stopped program because of exception");
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        [Route("api/Wallets/WithdrawWallet")]
        [HttpPost]
        public ActionResult<ResultDto> WithdrawWallet(RequestWithdrawWalletDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }

                return _mediator.Send(new WithdrawWalletCommand(request)).Result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Stopped program because of exception");
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
