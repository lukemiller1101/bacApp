﻿using AlcoholApp.Infrastructure;
using AlcoholApp.Models;
using AlcoholApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlcoholApp.Services
{
    public class GlassesService
    {
        private GlassesRepository _repo;
        private AppUsersRepository _AppUserRepo;
        private AlcoholsRepository _AlcRepo;

        public GlassesService(GlassesRepository repo, AppUsersRepository AppUserRepo, AlcoholsRepository AlcRepo)
        {
            _repo = repo;
            _AppUserRepo = AppUserRepo;
            _AlcRepo = AlcRepo;
        }

       

        public List<string> GetAlcoholTypeVolumes(string type)
        {
            var beer = new List<string> { "8 oz (Can)", "12 oz","16 oz" };
            var spirit = new List<string> { "1.5 oz Shot", "3 oz Double Shot"};
            var wine = new List<string> { "5 oz", "12 oz", "16 oz" };

            switch (type)
            {
                case "Beer":
                    return beer;
                case "Spirit":
                    return spirit;
                case "Wine":
                    return wine;
                default:
                    return wine;
            }
        }

        public string GetDrinkIcons(string type)
        {
            var beerI = "http://i.imgur.com/Vjrcg7e.png";
            var spiritI = "http://i.imgur.com/sHH0r9w.png";
            var wineI = "http://i.imgur.com/csqRohq.png";

            switch (type)
            {
                case "Beer":
                    return beerI;
                case "Spirit":
                    return spiritI;
                case "Wine":
                    return wineI;
                default:
                    return beerI;
                
            }


        }

        public void AddFav(int alcoholId, string userName)
        {
            var listFav = _repo.GetGlassByUser(userName);
            var user = _repo.GetUserByUserName(userName).FirstOrDefault();
            var favorites = _repo.List();

            var newFav = new Glass
            {
                IsFavorite = true,
                UserId = user.Id,
                AlcoholId = alcoholId
            };
            _repo.Add(newFav);
        }


        //public bool Check(string userName)
        //{
        //    var listFav = _repo.GetGlassByUser(userName);
        //    return (listFav.Count() < 4);
        //}


        public IEnumerable<AlcoholDTO> GetFavorites(string userName)
        {

            var favorites = (from f in _repo.List()
                             where f.AppUser.UserName == userName
                             select new AlcoholDTO
                             {
                                 Id = f.Alcohol.Id,
                                 ABV = f.Alcohol.ABV,
                                 Brand = f.Alcohol.Brand,
                                 Style = f.Alcohol.Style,
                                 Type = f.Alcohol.Type,
                                 Volumes = GetAlcoholTypeVolumes(f.Alcohol.Type)
                             });

            return favorites;
        }

        public IQueryable<Alcohol> GetRelations(string userName)
        {
            return (from f in _repo.List()
                    where f.AppUser.UserName == userName
                    select f.Alcohol);
        }



        public void DeleteFav(string userId, int alcId)
        {
            var favorite = _repo.GetGlassById(userId, alcId);
            _repo.Delete(favorite);
        }

        //Transferred Methods
        //Get
        public IEnumerable<GlassDTO> GetGlassDtos(string userName)
        {
            var glasses = (from g in _repo.List()
                           select new GlassDTO
                           {

                               TimeConsumed = g.TimeConsumed,
                               Volume = g.Volume,
                               Alcohol = new AlcoholDTO
                               {
                                   ABV = g.Alcohol.ABV,
                                   Brand = g.Alcohol.Brand,
                                   Id = g.Alcohol.Id,
                                   Style = g.Alcohol.Style,
                                   Type = g.Alcohol.Type,
                               }
                           });
            return glasses;
        }

        ////GetGlassFalse
        public IEnumerable<GlassDTO> GetGlassByUserNotFavorite(string userName)
        {
            var falseGlasses = (from fg in _repo.GetGlassByUserNotFavorite(userName)
                                select new GlassDTO
                                {
                                    Id = fg.Id,
                                    TimeConsumed = fg.TimeConsumed,
                                    Volume = fg.Volume,
                                    IsFavorite = fg.IsFavorite,
                                    Alcohol = new AlcoholDTO
                                    {
                                        ABV = fg.Alcohol.ABV,
                                        Brand = fg.Alcohol.Brand,
                                        Id = fg.Alcohol.Id,
                                        Style = fg.Alcohol.Style,
                                        Type = fg.Alcohol.Type,
                                        Icon = GetDrinkIcons(fg.Alcohol.Type),
                                        Volumes = GetAlcoholTypeVolumes(fg.Alcohol.Type)
                                    }
                                }).ToList();
            return falseGlasses;
        }

        //GetGlassTrue
        public IEnumerable<GlassDTO> GetGlassByUserTrue(string userName)
        {
            var trueGlasses = (from tg in _repo.GetGlassByUserFavorite(userName)
                               select new GlassDTO
                               {
                                   Id = tg.Id,
                                   TimeConsumed = tg.TimeConsumed,
                                   Volume = tg.Volume,
                                   IsFavorite = tg.IsFavorite,
                                   Alcohol = new AlcoholDTO
                                   {
                                       ABV = tg.Alcohol.ABV,
                                       Brand = tg.Alcohol.Brand,
                                       Id = tg.Alcohol.Id,
                                       Style = tg.Alcohol.Style,
                                       Type = tg.Alcohol.Type
                                   }
                               }).ToList();
            return trueGlasses;
        }


        //Add
        public void Add(string userId, string volume, int alcId)
        {
            var stringVol = volume.Split(' ').ToList();
     
           
            var glass = new Glass
            {
                Volume = Double.Parse(stringVol[0]),
                TimeConsumed = DateTime.Now,
                Alcohol = _AlcRepo.GetById(alcId).FirstOrDefault(),
                AppUser = _AppUserRepo.GetUserByUserName(userId).FirstOrDefault(),
                IsFavorite = false

            };
            _repo.Add(glass);
        }


        public void DeleteFalseGlass(string userId, int glassId)
        {
            var glass = _repo.GetNDeleteFalse(userId, glassId);
            _repo.Delete(glass);
        }

        public void DeleteAllFalseGlasses(string userName)
        {
            
            var falseGlasses = _repo.DeleteAllFalseGlasses(userName).ToList();
            for (int i = 0; i < falseGlasses.Count(); i++)
            {
                _repo.Delete(falseGlasses[i]);
            }
           
        }
    }
}
