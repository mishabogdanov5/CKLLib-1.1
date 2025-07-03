using CKLLib.Operations;
using CKLLib;

namespace CKLLibTests
{
	[TestClass]
	public class CKLGraphTests
	{
		
		private CKL _ckl = new CKL();
		private CKLGraph _graph;


		[TestInitialize]
		public void CKLGraphInit()
		{
			HashSet<string> A = ["a1", "a2", "a3"];
			HashSet<string> B = ["b1", "b2", "b3", "b4"];
			HashSet<string> C = ["c1", "c2"];

			HashSet<Pair> source = new HashSet<Pair>();

			foreach (string a in A)
			{
				foreach (string b in B)
				{
					foreach (string c in C) source.Add(new Pair(new List<object>() { a, b, c }));
				}
			}

			_ckl = new CKL()
			{
				FilePath = "C:\\Users\\graph_test2.ckl",
				GlobalInterval = new TimeInterval(0, 2000),
				Dimention = TimeDimentions.MINUTES,
				Source = source,
				Relation = [
					new RelationItem(new Pair(["a1", "b1", "c1"]), [new TimeInterval(200, 700), new TimeInterval(1400, 1850)]),
					new RelationItem(new Pair(["a1", "b1", "c2"]), [new TimeInterval(0, 1000)]),
					new RelationItem(new Pair(["a1", "b2", "c2"]), [new TimeInterval(1500, 1800), new TimeInterval(1900, 2000)]),
					new RelationItem(new Pair(["a1", "b3", "c1"]), [new TimeInterval(800, 1000), new TimeInterval(1200, 1950)]),
					new RelationItem(new Pair(["a2", "b2", "c1"]), [new TimeInterval(0, 200), new TimeInterval(1400, 1600)]),
					new RelationItem(new Pair(["a2", "b2", "c2"]), [new TimeInterval(0, 500), new TimeInterval(800, 1200),
						new TimeInterval(1800, 2000)]),
					new RelationItem(new Pair(["a2", "b3", "c2"]), [new TimeInterval(0, 300), new TimeInterval(400, 650),
					new TimeInterval(1000, 1300), new TimeInterval(1550, 1700)]),
					new RelationItem(new Pair(["a2", "b4", "c1"]), [new TimeInterval(600, 1100), new TimeInterval(1500, 2000)]),
					new RelationItem(new Pair(["a2", "b4", "c2"]), [new TimeInterval(0, 900), new TimeInterval(1200, 2000)]),
					new RelationItem(new Pair(["a3", "b1", "c1"]), [new TimeInterval(0, 200), new TimeInterval(900, 1600)]),
					new RelationItem(new Pair(["a3", "b1", "c2"]), [new TimeInterval(200, 700), new TimeInterval(900, 1050),
					new TimeInterval(1200, 1450), new TimeInterval(1600, 1900)]),
					new RelationItem(new Pair(["a3", "b2", "c1"]), [new TimeInterval(0, 700), new TimeInterval(1000, 1200)]),
					new RelationItem(new Pair(["a3", "b3", "c2"]), [new TimeInterval(100, 300), new TimeInterval(500, 800),
					new TimeInterval(900, 1250), new TimeInterval(1400, 1600), new TimeInterval(1700, 1950)]),
					new RelationItem(new Pair(["a3", "b4", "c1"]), [new TimeInterval(0, 1300), new TimeInterval(1600, 2000)])

				]
			};

			_graph = new CKLGraph(_ckl);
		}

		[TestMethod]
		public void TestCKLGraph_1()
		{
			HashSet<Pair> res = _graph.GetGraphByTime(1200);

			HashSet<Pair> exp = [new Pair(["a1", "b3", "c1"]), new Pair(["a2", "b3", "c2"]),
				new Pair(["a2", "b4", "c2"]), new Pair(["a3", "b1", "c1"]), new Pair(["a3", "b1", "c2"]),
				new Pair(["a3", "b3", "c2"]), new Pair(["a3", "b4", "c1"])
			];

			Assert.IsTrue(CKLMath.SourceEquality(res, exp));
		}

		[TestMethod]
		public void TestCKLGraph_2()
		{
			HashSet<Pair> res = _graph.GetGraphByTime(0);

			HashSet<Pair> exp = [new Pair(["a1", "b1", "c2"]), new Pair(["a2", "b2", "c1"]),
				new Pair(["a2", "b2", "c2"]), new Pair(["a2", "b3", "c2"]), new Pair(["a2", "b4", "c2"]),
				new Pair(["a3", "b1", "c1"]), new Pair(["a3", "b4", "c1"]), new Pair(["a3", "b2", "c1"])
			];


			Assert.IsTrue(CKLMath.SourceEquality(res, exp));
		}


		[TestMethod]
		public void TestCKLGraph_3()
		{
			HashSet<Pair> res = _graph.GetGraphByTime(2000);

			HashSet<Pair> exp = [ new Pair(["a3", "b4", "c1"]), new Pair(["a1", "b2", "c2"]),
				new Pair(["a2", "b4", "c1"]), new Pair(["a2", "b4", "c2"]), new Pair(["a2", "b2", "c2"])
			];

			Assert.IsTrue(CKLMath.SourceEquality(res, exp));
		}

		[TestMethod]
		public void TestCKLGraph_4()
		{
			HashSet<Pair> res = _graph.GetGraphByTime(1215.735);

			HashSet<Pair> exp = [new Pair(["a1", "b3", "c1"]), new Pair(["a2", "b3", "c2"]),
				new Pair(["a2", "b4", "c2"]), new Pair(["a3", "b1", "c1"]), new Pair(["a3", "b1", "c2"]),
				new Pair(["a3", "b3", "c2"]), new Pair(["a3", "b4", "c1"])
			];

			Assert.IsTrue(CKLMath.SourceEquality(res, exp));
		}
	}
}
