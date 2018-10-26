using System.Linq;
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
	public class ScalarTest
	{
		class TestTable
		{
			[PrimaryKey, AutoIncrement]
			public int Id { get; set; }
			public int Two { get; set; }
		}

		const int Count = 100;

		SQLiteConnection CreateDb ()
		{
			var db = new TestDb ();
			db.CreateTable<TestTable> ();
			var items = from i in Enumerable.Range (0, Count)
				select new TestTable { Two = 2 };
			db.InsertAll (items);
			Assert.AreEqual (Count, db.Table<TestTable> ().Count ());
			return db;
		}


		[Test]
		public void Int32 ()
		{
			var db = CreateDb ();
			
			var r = db.ExecuteScalar<int> ("SELECT SUM(Two) FROM TestTable");

			Assert.AreEqual (Count * 2, r);
		}

		[Test]
		public void SelectSingleRowValue ()
		{
			var db = CreateDb ();

			var r = db.ExecuteScalar<int> ("SELECT Two FROM TestTable WHERE Id = 1 LIMIT 1");

			Assert.AreEqual (2, r);
		}
	}
}
