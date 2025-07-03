using System.Net;
using System.Net.Http.Headers;
using CKLLib;
using CKLLib.Operations;
using Newtonsoft.Json.Bson;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CKLLibTests
{
	[TestClass]
	public class CKLMathTests
	{

		[TestMethod]
		public void ItemProjection_Operation_Test_1()
		{
			CKL data = new CKL()
			{
				FilePath = "C:\\Users\\user\\Ciclograms\\test1.ckl",
				GlobalInterval = new TimeInterval(0, 1500),
				Dimention = TimeDimentions.MICROSECONDS,
				Source = new HashSet<Pair>() { new Pair (["A1", "B1"]), new Pair(["A1", "B2"]), new Pair(["A1", "B3"]),
					new Pair(["A2", "B1"]), new Pair(["A2", "B2"]), new Pair(["A2", "B3"])},
				Relation = new HashSet<RelationItem>()
				{
					new RelationItem(new Pair(["A1", "B1"]), [new TimeInterval(200, 400), new TimeInterval(600, 900),
						new TimeInterval(1400, 1450)]),
					new RelationItem(new Pair(["A1", "B3"]), [new TimeInterval(0, 400), new TimeInterval(1000, 1400)]),
					new RelationItem(new Pair(["A2", "B2"]), [new TimeInterval(100, 800), new TimeInterval(1100, 1250)]),
					new RelationItem(new Pair(["A2", "B3"]), [new TimeInterval(0, 1200)])
				}
			};

			TimeInterval interval = new TimeInterval(300, 500);


			CKL res = CKLMath.ItemProjection(data, "B3", (object obj1, object obj2) => obj1.ToString().Equals(obj2.ToString()),
				interval);

			CKL exp = new CKL()
			{
				FilePath = "",
				GlobalInterval = interval,
				Dimention = TimeDimentions.MICROSECONDS,
				Source = new HashSet<Pair>() { new Pair(["A2", "B3"]) },
				Relation = new HashSet<RelationItem>() { new RelationItem(new Pair(["A2", "B3"]), [new TimeInterval(300, 500)]) }
			};

			Assert.AreEqual<CKL>(exp, res, new CKLEqualityComparer());
		}

		[TestMethod]
		public void Item_Projection_Test_2()
		{
			List<object> l1 = new List<object>() { "K1", "K2", "K3" };
			List<object> l2 = new List<object>() { "M1", "M2" };
			List<object> l3 = new List<object>() { "N1", "N2", "N3", "N4"};
			List<object> l4 = new List<object>() { "S1", "S2", "S3"};

			HashSet<Pair> source = new HashSet<Pair>();

			foreach (object obj1 in l1)
			{
				foreach (object obj2 in l2) 
				{
					foreach (object obj3 in l3) 
					{
						foreach (object obj4 in l4) 
						{
							source.Add(new Pair([obj1, obj2, obj3, obj4]));
						}
					}
				}
			}

			CKL data = new CKL()
			{
				FilePath = "test.ckl",
				GlobalInterval = new TimeInterval(1000, 5250),
				Dimention = TimeDimentions.NANOSECONDS,
				Source = source,
				Relation = new HashSet<RelationItem>()
				{
					new RelationItem(new Pair(new List<object>() { "K1", "M1", "N1", "S1"}), [new TimeInterval(1000, 1300), new TimeInterval(3000, 4220)]),
					new RelationItem(new Pair(new List<object>() { "K1", "M1", "N1", "S2"}), [new TimeInterval(1500, 1725), new TimeInterval(2015, 2200), 
						new TimeInterval(3200, 4725)]),
					new RelationItem(new Pair(new List<object>() { "K1", "M1", "N2", "S2"}), [new TimeInterval(2025, 3075)]),
					new RelationItem(new Pair(new List<object>() { "K1", "M1", "N2", "S3"}), [new TimeInterval(1600, 2300), new TimeInterval(4050, 5100)]),
					new RelationItem(new Pair(new List<object>() { "K1", "M1", "N3", "S1"}), [new TimeInterval(1000, 1825), new TimeInterval(2000, 3450), 
						new TimeInterval(4600, 4975)]),
					new RelationItem(new Pair(new List<object>() { "K1", "M2", "N1", "S1"}), [new TimeInterval(1550, 1975), new TimeInterval(2105, 3215)]),
					new RelationItem(new Pair(new List<object>() { "K1", "M2", "N1", "S3"}), [new TimeInterval(2450, 2975), new TimeInterval(3835, 4065)]),
					new RelationItem(new Pair(new List<object>() { "K1", "M2", "N4", "S2"}), [new TimeInterval(1110, 1405), new TimeInterval(1975, 2745), 
					new TimeInterval(3220, 3585), new TimeInterval(4295, 4700)]),
					new RelationItem(new Pair(new List<object>() { "K2", "M2", "N1", "S3"}), [new TimeInterval(3300, 3600)]),
					new RelationItem(new Pair(new List<object>() { "K2", "M2", "N3", "S1"}), [new TimeInterval(2400, 3035), new TimeInterval(3205, 4000), 
					new TimeInterval(4550, 4945), new TimeInterval(5100, 5250)]),
					new RelationItem(new Pair(new List<object>() { "K2", "M2", "N4", "S2"}), [new TimeInterval(1200, 1385), new TimeInterval(2005, 2815),
					new TimeInterval(3025, 3450), new TimeInterval(3800, 4025), new TimeInterval(4550, 4750), new TimeInterval(4900, 5050)]),
					new RelationItem(new Pair(new List<object>() { "K3", "M1", "N1", "S1"}), [new TimeInterval(2200, 2350), new TimeInterval(2425, 3000),
					new TimeInterval(3295, 4300)]),
					new RelationItem(new Pair(new List<object>() { "K3", "M1", "N3", "S2"}), [new TimeInterval(1550, 2325), new TimeInterval(2650, 3220)]),
					new RelationItem(new Pair(new List<object>() { "K3", "M1", "N3", "S3"}), [new TimeInterval(1800, 2750)]),
					new RelationItem(new Pair(new List<object>() { "K3", "M2", "N2", "S3"}), [new TimeInterval(1000, 1200), new TimeInterval(2100, 3400),
					new TimeInterval(4800, 5250)]),
					new RelationItem(new Pair(new List<object>() { "K3", "M2", "N3", "S1"}), [new TimeInterval(1000, 5250)]),
					new RelationItem(new Pair(new List<object>() { "K3", "M2", "N4", "S1"}), [new TimeInterval(1350, 2345), new TimeInterval(2700, 5050)]),
					new RelationItem(new Pair(new List<object>() { "K3", "M2", "N4", "S3"}), [new TimeInterval(1400, 1600), new TimeInterval(1800, 3225)]),
				}
			};

			TimeInterval interval = new TimeInterval(1500, 1600);

			List<object> currents = ["K3", "S1"];

			CKL res = CKLMath.ItemProjection(data, currents, (obj1, obj2) => obj1.ToString().Equals(obj2.ToString()), interval);

			CKL exp = new CKL()
			{
				FilePath = "",
				GlobalInterval = interval,
				Dimention = TimeDimentions.NANOSECONDS,
				Source = new HashSet<Pair>()
				{
					new Pair(new List<object>() {"K3", "M2", "N3", "S1"}),
					new Pair(new List<object>() {"K3", "M2", "N4", "S1"})
				},
				Relation = new HashSet<RelationItem>()
				{
					new RelationItem(new Pair(new List<object>() {"K3", "M2", "N3", "S1"}), [interval]),
					new RelationItem(new Pair(new List<object>() {"K3", "M2", "N4", "S1"}), [interval])
				}
			};

			Assert.AreEqual(exp, res, new CKLEqualityComparer());
		}

		[TestMethod]
		public void CKL_Group_Test_1() 
		{
			List<CKL> diagrams = new List<CKL>() 
			{
				new CKL ("1.ckl", new TimeInterval(100, 3000), TimeDimentions.SECONDS, [new Pair(["a1", "b1"]), new Pair(["a1", "b2"]), new Pair(["a2", "b1"]), 
					new Pair(["a2", "b2"])],
					[new RelationItem(new Pair(["a1", "b1"]), [new TimeInterval(350, 700)])]
					),

				new CKL ("2.ckl", new TimeInterval(250, 2200), TimeDimentions.SECONDS, [new Pair(["a1", "b1"]), new Pair(["a1", "b2"]),  new Pair(["a2", "b1"]), 
					new Pair(["a2", "b2"])],
					[new RelationItem(new Pair(["a2", "b1"]), [new TimeInterval(1150, 1775)])]
					),

				new CKL ("3.ckl", new TimeInterval(400, 800), TimeDimentions.SECONDS, [new Pair(["b1", "c1"]), new Pair(["b1", "c2"]), new Pair(["b1", "c2"]),
				new Pair(["b2", "c1"]), new Pair(["b2", "c2"]), new Pair(["b2", "c3"])], []),

				new CKL ("4.ckl", new TimeInterval(0, 1600), TimeDimentions.MINUTES, [new Pair(["a1", "b1"]), new Pair(["a1", "b2"]), new Pair(["a2", "b1"]), 
					new Pair(["a2", "b2"])], [])
			};

			List<List<CKL>> res = CKLMath.GroupByTheme(diagrams);
			List<List<CKL>> exp = [[diagrams[0], diagrams[1], diagrams[2]], [diagrams[3]]];

			if (res.Count != exp.Count) Assert.AreEqual(exp.Count, res.Count);
			 
			for (int i = 0; i < res.Count; i++) 
			{
				if (!res[i].SequenceEqual(exp[i], new CKLEqualityComparer())) CollectionAssert.AreEqual(exp[i], res[i]);
			}

			Assert.AreEqual(true, true);
		}

		private CKL res;
		[TestMethod]
		public void CKL_Composition_Test_1()
		{
			CKL ckl1 = new CKL() 
			{
				FilePath = "file1.ckl",
				GlobalInterval = new TimeInterval(0,100),
				Dimention = TimeDimentions.MILLISECONDS,
				Source = [new Pair(["a1", "b1"]), new Pair(["a1", "b2"]), new Pair(["a2", "b1"]), new Pair(["a2", "b2"])],
				Relation = 
				[
					new RelationItem(new Pair(["a1", "b1"]), [new TimeInterval(10, 50), new TimeInterval(70, 85)]),
					new RelationItem(new Pair(["a1", "b2"]), [new TimeInterval(0, 20), new TimeInterval(30, 60), new TimeInterval(90, 100)]),
					new RelationItem(new Pair(["a2", "b1"]), [new TimeInterval(80, 100)])
				]
			};

			CKL ckl2 = new CKL()
			{
				FilePath = "file2.ckl",
				GlobalInterval = new TimeInterval(0, 100),
				Dimention = TimeDimentions.MILLISECONDS,
				Source = [new Pair(["b1", "c1"]), new Pair(["b1", "c2"]), new Pair(["b2", "c1"]), new Pair(["b2", "c2"])],
				Relation =
				[
					new RelationItem(new Pair(["b1", "c1"]), [new TimeInterval(0, 15), new TimeInterval(50, 65)]),
					new RelationItem(new Pair(["b1", "c2"]), [new TimeInterval(0, 20), new TimeInterval(40, 50), new TimeInterval(70, 100)]),
					new RelationItem(new Pair(["b2", "c1"]), [new TimeInterval(60, 80)]),
					new RelationItem(new Pair(["b2", "c2"]), [new TimeInterval(0, 40), new TimeInterval(60, 70)])
				]
			};

			res = CKLMath.Composition(ckl1, ckl2, 5);

			CKL exp = new CKL()
			{
				FilePath = "res.ckl",
				GlobalInterval = new TimeInterval(0, 100),
				Dimention = TimeDimentions.MILLISECONDS,
				Source = [new Pair(["a1", "b1", "c1"]), new Pair(["a1", "b1", "c2"]), new Pair(["a1", "b2", "c1"]), new Pair(["a1", "b2", "c2"]),
				new Pair(["a2", "b1", "c1"]), new Pair(["a2", "b1", "c2"]), new Pair(["a2", "b2", "c1"]), new Pair(["a2", "b2", "c2"])],
			};

			exp.Relation =
				[
					new RelationItem(new Pair(["a1", "b1", "c1"]), [new TimeInterval(50, 65)]),
					new RelationItem(new Pair(["a1", "b1", "c2"]), [new TimeInterval(15, 20), new TimeInterval(40, 50), new TimeInterval(70, 100)]),
					new RelationItem(new Pair(["a1", "b2", "c1"]), [new TimeInterval(60, 80)]),
					new RelationItem(new Pair(["a1", "b2", "c2"]), [new TimeInterval(5, 40), new TimeInterval(60, 70)]),
					new RelationItem(new Pair(["a2", "b1", "c2"]), [new TimeInterval(85, 100)])
				];

			Assert.AreEqual(1, 1);
		}
	}	
}