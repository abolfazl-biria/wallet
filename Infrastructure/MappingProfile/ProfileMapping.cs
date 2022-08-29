using Application.Services.Wallets.Queries.GetAllWallets;
using Application.Services.Wallets.Queries.GetWalletById;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.MappingProfile
{
    public class ProfileMapping : Profile
    {
        public ProfileMapping()
        {
            CreateMap<Wallet, ResultGetAllWalletsDto>();

            CreateMap<Wallet, ResultGetWalletByIdDto>();
        }
    }
}
