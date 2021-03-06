﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mapster.Tests.Classes;
using NUnit.Framework;
using Should;

namespace Mapster.Tests
{
    [TestFixture]
    public class WhenProjecting
    {
        [Test]
        public void TestTypeConversion()
        {
            TypeTestClassA testA = new TypeTestClassA();
            testA.A = 5;
            testA.B = 2;
            testA.C = 4.5;

            var list = new List<TypeTestClassA>() { testA };

            var bList = list.AsQueryable().Project().To<TypeTestClassB>().ToList();

            Assert.IsNotNull(bList);

            Assert.IsTrue(bList.Count == 1);
            Assert.IsTrue(bList[0].A == 5);
            Assert.IsTrue(bList[0].B == 2);
            Assert.IsTrue(bList[0].C == 4.5m);
        }

        [Test]
        public void TestProjectionConfiguration()
        {
            ConfigTestClassA testA = new ConfigTestClassA();
            testA.A = 5;
            testA.B = "2";
            testA.C = 4.5;

            var list = new List<ConfigTestClassA>() { testA };
            
            TypeAdapterConfig<ConfigTestClassA, ConfigTestClassB>
                .NewConfig()
                .Ignore(dest => dest.A)
                .Map(dest => dest.B, src => Convert.ToInt32(src.B))
                .Map(dest => dest.C, src => src.C.ToString());

            var bList = list.AsQueryable().Project().To<ConfigTestClassB>().ToList();

            Assert.IsNotNull(bList);

            Assert.IsTrue(bList.Count == 1);
            Assert.IsTrue(bList[0].A == null);
            Assert.IsTrue(bList[0].B == int.Parse(testA.B));
            Assert.IsTrue(bList[0].C == testA.C.ToString());
        }

        [Test]
        public void TestPocoTypeMapping()
        {
            var products = new[]
            {
                new Product {Id = Guid.NewGuid(), Title = "ProductA", CreatedUser = new User {Name = "UserA"}},
                new Product {Id = Guid.NewGuid(), Title = "ProductB", CreatedUser = null},
            };

            var resultQuery = products.AsQueryable().Project().To<ProductDTO>();
            var expectedQuery = from Param_0 in products.AsQueryable()
                                select new ProductDTO
                                {
                                    Id = Param_0.Id,
                                    Title = Param_0.Title,
                                    CreatedUserName = Param_0.CreatedUser.Name,
                                };
            resultQuery.ToString().ShouldEqual(expectedQuery.ToString());
        }
    }
}
