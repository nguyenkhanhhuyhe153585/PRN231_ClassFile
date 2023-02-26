﻿using AutoMapper;
using ClassFileBackEnd.Authen;
using ClassFileBackEnd.Common;
using ClassFileBackEnd.Mapper;
using ClassFileBackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ClassFileBackEnd.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ClassController : Controller
    {
        private readonly ClassfileContext db;
        private readonly IMapper mapper;
        public ClassController(ClassfileContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                var currentUserId = JWTManagerRepository.GetClaim(JwtRegisteredClaimNames.Name, HttpContext);
                int currentId = int.Parse(currentUserId);
                Account currentUser = db.Accounts.Where(e=>e.Id == currentId).Include(a=>a.Classes).SingleOrDefault();
                List<Class>? clasese = currentUser.Classes.ToList();
                List<ClassDTO> classDTOs = mapper.Map<List<ClassDTO>>(clasese);
                return Ok(classDTOs);
            }
            catch (Exception ex) 
            {
                ResponseMessageDTO<string> responseMsg = new ResponseMessageDTO<string>(ex.Message);
                responseMsg.Data = ex.StackTrace;
                return BadRequest(responseMsg);
            }
        }
    }
}
