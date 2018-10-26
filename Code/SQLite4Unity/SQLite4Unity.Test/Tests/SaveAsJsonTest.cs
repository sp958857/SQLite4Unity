using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace SQLite4Unity.Test
{
    public class SaveAsJsonTest
    {
        public class DefalutConvert : IJsonConverter
        {
            public object FromBody(Type t, string body)
            {
                bool isError = false;
                string errorMessage = "";
                object data = JsonConvert.DeserializeObject(body, t,
                    new JsonSerializerSettings
                    {
                        Error = (o, args) =>
                        {
                            args.ErrorContext.Handled = true;
                            errorMessage = args.ErrorContext.Error.Message;
                            isError = true;
                        }
                    });
                if (isError)
                {
                    throw new Exception(errorMessage);
                }
                return data;
            }

            public string ToBody(object data)
            {
                string dataString = "";
                dataString = JsonConvert.SerializeObject(data);
                return dataString;
            }
        }
        public class Car
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }
            public string Name { get; set; }
            [SaveAsJson]
            public Quality Quality { get; set; }

            [SaveAsJson]
            public List<string> Tags { get; set; }
        }

        public class Quality
        {
            public int Speed { get; set; }
            public int Hardness { get; set; }

        }

        public class BrandContainer<T>
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }
            public string Name { get; set; }
            [SaveAsJson]
            public List<T> Types { get; set; }
        }

        public class TestDb : SQLiteConnection
        {
            public TestDb(String path, IJsonConverter jsonConverter)
                : base(path, jsonConverter)
            {
                CreateTable<Car>();
                CreateTable<BrandContainer<Car>>();
            }
        }

        private TestDb _db;
        public BrandContainer<Car> Chevrolet = new BrandContainer<Car>()
        {
            Name = "Chevrolet"
        };
        public BrandContainer<Car> Benz = new BrandContainer<Car>()
        {
            Name = "Benz"
        };
        public Car Cruze = new Car()
        {
            Name = "Cruze",
            Quality = new Quality()
            {
                Hardness = 80,
                Speed = 210
            },
            Tags = new List<string>()
            {
                "family car","economy car"
            }
        };
        public Car S600 = new Car()
        {
            Name = "S600",
            Quality = new Quality()
            {
                Hardness = 90,
                Speed = 320
            },
            Tags = new List<string>()
            {
                "business car","comfortable car"
            }
        };
        public Car C600 = new Car()
        {
            Name = "C600",
            Quality = new Quality()
            {
                Hardness = 85,
                Speed = 280
            },
            Tags = new List<string>()
            {
                "Millions of sales","comfortable car"
            }
        };

        [SetUp]
        public void Setup()
        {
            _db = new TestDb(TestPath.GetTempFileName(),new DefalutConvert());
        }
        [TearDown]
        public void TearDown()
        {
            if (_db != null) _db.Close();
        }

        [Test]
        public void ListAndCustomTypeTest()
        {
            _db.Insert(Cruze);
            _db.Insert(S600);
            _db.Insert(C600);
            Assert.AreEqual(3,_db.Table<Car>().Count());
            Assert.AreEqual(210,_db.Table<Car>().Where(c=>c.Name.Equals("Cruze")).FirstOrDefault().Quality.Speed);
        }

        [Test]
        public void GenericTypeTest()
        {
            Chevrolet.Types = new List<Car>(){Cruze};
            Benz.Types = new List<Car>(){S600, C600 };
            _db.Insert(Chevrolet);
            _db.Insert(Benz);
            Assert.AreEqual(2, _db.Table<BrandContainer<Car>>().Count());
            Assert.AreEqual(2, _db.Table<BrandContainer<Car>>().Where(b => b.Name.Equals("Benz")).FirstOrDefault().Types.Count);
            var benzTypes = _db.Table<BrandContainer<Car>>().Where(b => b.Name.Equals("Benz")).FirstOrDefault().Types;
            benzTypes.ForEach(x=>Console.WriteLine("benz type:"+x.Name));
            Assert.AreEqual(2, benzTypes.Count);

        }
    }
}