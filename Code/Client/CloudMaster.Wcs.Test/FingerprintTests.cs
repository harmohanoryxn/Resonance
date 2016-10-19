using System;
using System.Collections.Generic;
using System.Linq;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CloudMaster.Wcs.Test
{
	/// <summary>
	/// Summary description for FingerprintTest
	/// </summary>
	[TestClass]
	public class FingerprintTests
	{
		private Random _random;

		private List<Note> _notes;
		public FingerprintTests()
		{
			_random =new Random(1);
			_notes = new List<Note>();
			_notes.Add(CreateNote1());
		}

		public Note CreateNote1()
		{
			var note = new Note();
			note.NoteId = 1;
			note.Source = "aaaaaaaaaa";
			note.NoteMessage = "dfs fdf sdfp k,";
			note.DateCreated = DateTime.Parse("10 May 2010");
			note.LastUpdated = DateTime.Parse("12 May 2010");
			return   note;
		}
		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		public void NoteTest()
		{
			var note = CreateNote1();
			var fp1 = note.GetFingerprint();

			note.DateCreated = note.DateCreated.AddDays(1);
			var fp2 = note.GetFingerprint();

			note.DateCreated = note.DateCreated.AddDays(-1);
			var fp3 = note.GetFingerprint();
		}
	}
}
