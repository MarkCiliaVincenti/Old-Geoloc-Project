using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SmartFleet.Core.Domain.Customers;
using SmartFleet.Core.Domain.Users;
using SmartFleet.Core.Domain.Vehicles;

namespace SmartFleet.Data
{
    public class SeedInitData
    {
        public static void SeedInitialData()
        {

            string[] roles = { "admin", "customer", "user" };
            SmartFleetObjectContext context = new SmartFleetObjectContext();
            
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            for (int i = 0; i < roles.Length; i++)
            {
                if (roleManager.RoleExists(roles[i]) == false)
                {
                    roleManager.Create(new IdentityRole(roles[i]));
                }
            }

            // user

            var userManager = new UserManager<User>(new UserStore<User>(context));
            var passwordHash = new PasswordHasher();
            if (!context.Users.Any(u => u.UserName == "admin@smartFleet"))
            {
                var user = new User()
                {
                    UserName = "admin@smartFleet",
                    Email = "admin@smartfleet.net",
                    PasswordHash = passwordHash.HashPassword("123456")
                };

                userManager.Create(user);
                userManager.AddToRole(user.Id, roles[0]);
            }

            

            var customerId = Guid.NewGuid();
            if (!context.Customers.Any(u => u.Name == "smartFleet"))
            {
                var user = new Customer()
                {
                    Name = "smartFleet",
                    Email = "customer@smartfleet.net",
                    CreationDate = DateTime.Now,
                    Id = customerId

                };

                context.Customers.Add(user);
                context.SaveChanges();
            }
            if (!context.Users.Any(u => u.UserName == "customer@smartFleet"))
            {
                var user = new User()
                {
                    UserName = "customer@smartFleet",
                    Email = "customer@smartfleet.net",
                    PasswordHash = passwordHash.HashPassword("123456"),
                    CustomerId = customerId
                };

                userManager.Create(user);
                userManager.AddToRole(user.Id, roles[1]);
            }

            foreach (var modelName in GetModels().Select(x => x.Value).Distinct())
            {
                if (!context.Models.Any(m => m.Name == modelName))
                {
                    var entity = new Model { Name = modelName };
                    context.Models.Add(entity);
                }
            }

            context.SaveChanges();

        }
        private static Dictionary<string, string> GetModels()
        {
            var models = new Dictionary<string, string>();
            models.Add("JMB", "Mitsubishi");
            models.Add("JA", "Isuzu");
            models.Add("JF", "Fuji");
            models.Add("JH", "Honda");
            models.Add("JK", "Kawasaki");
            models.Add("JM", "Mazda");
            models.Add("JN", "Nissan");
            models.Add("JS", "Suzi");
            models.Add("JT", "Toyota");
            models.Add("KL", "Daewoo");
            models.Add("KM8", "Hyundai");
            models.Add("KMH", "Hyundai");
            models.Add("KNA", "Kia");
            models.Add("KNB", "Kia");
            models.Add("KNC", "Kia");
            models.Add("KNM", "Renault");
            models.Add("L56", "Renault");
            models.Add("L5Y", "MERATO");
            models.Add("LDY", "Zhongtong");
            models.Add("LKL", "Suzhou King");
            models.Add("LSY", "Brilliance");
            models.Add("LTV", "Toyota");
            models.Add("LVS", "Ford");
            models.Add("LZM", "MAN");
            models.Add("LZE", "Isuzu");
            models.Add("LZG", "Shaanxi");
            models.Add("LZY", "Yutong");
            models.Add("MA3", "Suzi");
            models.Add("NLE", "Mercedes-Benz");
            models.Add("NM4", "Fiat");
            models.Add("NMT", "Toyota");
            models.Add("S2D", "Chrysler");
            models.Add("SAL", "Land Rover");
            models.Add("SAJ", "Jaguar");
            models.Add("SAR", "Rover");
            models.Add("SB1", "Toyota");
            models.Add("SCC", "Lotus Cars");
            models.Add("SCE", "DeLorean Motor");
            models.Add("SDB", "Peugeot ");
            models.Add("SFD", "Alexander Dennis");
            models.Add("SHS", "Honda");
            models.Add("SJN", "Nissan");
            models.Add("TMB", "Skoda Auto");
            models.Add("TMT", "Tatra");
            models.Add("TRA", "Iris");
            models.Add("TRU", "Audi");
            models.Add("TSM", "Suzi");
            models.Add("UU1", "Renault");
            models.Add("VF1", "Renault");
            models.Add("VF3", "Peugeot");
            models.Add("VF4", "Talbot");
            models.Add("VF6", "Renault");
            models.Add("VF7", "Citroën");
            models.Add("VF8", "Matra");
            models.Add("VFA", "Renault");
            models.Add("VJ1", "Heuliez");
            models.Add("VJ2", "Mia");
            models.Add("VN1", "Opel");
            models.Add("VNV", "Nissan");
            models.Add("VNK", "Toyota");
            models.Add("VR1", "DS");
            models.Add("VSS", "SEAT");
            models.Add("VSX", "Opel");
            models.Add("VS6", "Ford");
            models.Add("VSG", "Nissan");
            models.Add("VSE", "Santana");
            models.Add("VWV", "Volkswagen");
            models.Add("WAU", "Audi");
            models.Add("WUA", "Quattro");
            models.Add("WBA", "BMW");
            models.Add("WBS", "BMW");
            models.Add("WDB", "Mercedes-Benz");
            models.Add("WDC", "DaimlerChrysler");
            models.Add("WDD", "McLaren");
            models.Add("WF0", "Ford");
            models.Add("WMA", "MAN");
            models.Add("WMW", "MINI");
            models.Add("WP0", "Porsche");
            models.Add("WP1", "Porsche");
            models.Add("W0L", "Opel");
            models.Add("W0V", "Opel");
            models.Add("WVW", "Volkswagen");
            models.Add("WV1", "Volkswagen");
            models.Add("WV2", "Volkswagen");
            models.Add("XL9", "Spyker");
            models.Add("XLR", "DAF");
            models.Add("XTA", "Lada");
            models.Add("YK1", "Saab");
            models.Add("YS2", "Scania");
            models.Add("YS3", "Saab");
            models.Add("YV1", "Volvo");
            models.Add("YV4", "Volvo");
            models.Add("YV2", "Volvo");
            models.Add("YV3", "Volvo");
            models.Add("ZAM", "Maserati");
            models.Add("ZAP", "Piaggio/Vespa/Gilera");
            models.Add("ZAR", "Alfa Romeo");
            models.Add("ZCF", "Iveco");
            models.Add("ZCG", "Cagiva");
            models.Add("ZDM", "Ducat");
            models.Add("ZDF", "Ferrari");
            models.Add("ZD4", "Aprilia");
            models.Add("ZFA", "Fiat");
            models.Add("ZFC", "Fiat");
            models.Add("ZFF", "Ferrari");
            models.Add("ZHW", "Lamborghini");
            models.Add("ZLA", "Lancia");
            models.Add("ZOM", "OM");
            models.Add("1C3", "Chrysler");
            models.Add("1D3", "Dodge");
            models.Add("1FA", "Ford");
            models.Add("1FB", "Ford");
            models.Add("1FC", "Ford");
            models.Add("1FD", "Ford");
            models.Add("1FM", "Ford");
            models.Add("1FT", "Ford");
            models.Add("1FU", "Freightliner");
            models.Add("1FV", "Freightliner");
            models.Add("1F9", "FWD Corp.");
            models.Add("1G", "General Motors");
            models.Add("1GC", "Chevrolet");
            models.Add("1GT", "GMC");
            models.Add("1G1", "Chevrolet");
            models.Add("1G2", "Pontiac");
            models.Add("1G3", "Oldsmobile");
            models.Add("1G4", "Buick");
            models.Add("1G6", "Cadillac");
            models.Add("1GM", "Pontiac");
            models.Add("1G8", "Saturn");
            models.Add("1H", "Honda");
            models.Add("1HD", "Harley-Davidson");
            models.Add("1J4", "Jeep");
            models.Add("1L", "Lincoln");
            models.Add("1ME", "Mercury");
            models.Add("1M1", "Mack");
            models.Add("1M2", "Mack");
            models.Add("1M3", "Mack");
            models.Add("1M4", "Mack");
            models.Add("1N", "Nissan");
            models.Add("1NX", "NUMMI");
            models.Add("1P3", "Plymouth");
            models.Add("1R9", "Roadrunner");
            models.Add("1VW", "Volkswagen");
            models.Add("1XK", "Kenworth");
            models.Add("1XP", "Peterbilt");
            models.Add("1YV", "Mazda");
            models.Add("2C3", "Chrysler");
            models.Add("2D3", "Dodge ");
            models.Add("2FA", "Ford");
            models.Add("2FB", "Ford");
            models.Add("2FC", "Ford");
            models.Add("2FM", "Ford");
            models.Add("2FT", "Ford");
            models.Add("2FU", "Freightliner");
            models.Add("2FV", "Freightliner");
            models.Add("2FZ", "Sterling");
            models.Add("2G", "General Motors");
            models.Add("2G1", "Chevrolet");
            models.Add("2G2", "Pontiac");
            models.Add("2G3", "Oldsmobile");
            models.Add("2G4", "Buick");
            models.Add("2HG", "Honda");
            models.Add("2HK", "Honda");
            models.Add("2HM", "Hyundai");
            models.Add("2M", "Mercury");
            models.Add("2P3", "Plymouth");
            models.Add("2T", "Toyota");
            models.Add("2WK", "Western Star");
            models.Add("2WL", "Western Star");
            models.Add("2WM", "Western Star");
            models.Add("3D3", "Dodge");
            models.Add("3FE", "Ford");
            models.Add("3G", "General Motors");
            models.Add("3H", "Honda");
            models.Add("3N", "Nissan");
            models.Add("3P3", "Plymouth");
            models.Add("3VW", "Volkswagen");
            models.Add("4F", "Mazda");
            models.Add("4M", "Mercury");
            models.Add("4S", "Subaru-Isuzu");
            models.Add("4T", "Toyota");
            models.Add("4US", "BMW");
            models.Add("4UZ", "Frt-Thomas");
            models.Add("4V1", "Volvo");
            models.Add("4V2", "Volvo");
            models.Add("4V3", "Volvo");
            models.Add("4V4", "Volvo");
            models.Add("4V5", "Volvo");
            models.Add("4V6", "Volvo");
            models.Add("4VL", "Volvo");
            models.Add("4VM", "Volvo");
            models.Add("4VZ", "Volvo");
            models.Add("5F", "Honda");
            models.Add("5L", "Lincoln");
            models.Add("5N1", "Nissan ");
            models.Add("5NP", "Hyundai");
            models.Add("5T", "Toyota");
            models.Add("6F", "Ford");
            models.Add("6G2", "Pontiac");
            models.Add("6H", "General Motors");
            models.Add("6MM", "Mitsubishi");
            models.Add("6T1", "Toyota Motor ");
            models.Add("8A1", "Renault");
            models.Add("8AG", "Chevrolet");
            models.Add("8GG", "Chevrolet");
            models.Add("8AP", "Fiat");
            models.Add("8AF", "Ford");
            models.Add("8AD", "Peugeot");
            models.Add("8GD", "Peugeot");
            //models.Add("8A1", "Renault ");
            models.Add("8AK", "Suzi");
            models.Add("8AJ", "Toyota");
            models.Add("8AW", "Volkswagen");
            models.Add("93U", "Audi");
            models.Add("9BG", "Chevrolet");
            models.Add("935", "Citroën");
            models.Add("9BD", "Fiat");
            models.Add("9BF", "Ford");
            models.Add("93H", "Honda");
            models.Add("9BM", "Mercedes-Benz");
            models.Add("936", "Peugeot");
            models.Add("93Y", "Renault");
            models.Add("9BS", "Scania");
            models.Add("93R", "Toyota");
            models.Add("9BW", "Volkswagen");
            models.Add("9FB", "Renault");
            return models;
        }
    }
}
