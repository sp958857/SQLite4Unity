using NUnit.Framework;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using SetUp = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestInitializeAttribute;
using TestFixture = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
#else

#endif


namespace SQLite4Unity.Test
{
	[TestFixture]
	public class DropTableTest
	{
		public class Product
		{
			[AutoIncrement, PrimaryKey]
			public int Id { get; set; }
			public string Name { get; set; }
			public decimal Price { get; set; }
		}
		
		public class TestDb : SQLiteConnection
		{
			public TestDb () : base(TestPath.GetTempFileName ())
			{
				Trace = true;
			}
		}
		
		[Test]
		public void CreateInsertDrop ()
		{
			var db = new TestDb ();
			
			db.CreateTable<Product> ();
			
			db.Insert (new Product {
				Name = "Hello",
				Price = 16,
			});
			
			var n = db.Table<Product> ().Count ();
			
			Assert.AreEqual (1, n);
			
			db.DropTable<Product> ();
			
			ExceptionAssert.Throws<SQLiteException>(() => db.Table<Product> ().Count ());
		}
	}
}
