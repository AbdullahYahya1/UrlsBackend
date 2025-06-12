using AutoMapper;
using System;
using System.Net.Mail;
using System.Net.Sockets;
using System.Numerics;
using System.Xml.Linq;
using UrlsBackend.Data.Models;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, GetUserDto>().ReverseMap();

    }
}
 