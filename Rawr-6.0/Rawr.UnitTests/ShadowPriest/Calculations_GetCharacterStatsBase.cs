﻿using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rawr;
//using Rawr.ShadowPriest;

namespace Rawr.UnitTests.ShadowPriest
{
    /// <summary>
    /// A base class for testing the <see cref="CalculationsShadowPriest"/> GetCharacterStats method with different character setups
    /// </summary>
    public abstract class Calculations_GetCharacterStatsBase
    {
        #region Fields

        protected Character _character;

        #endregion

        #region Methods

//        [TestFixtureSetUp]
//        public void SetupFixture()
//        {
            /*using (var fileStream = File.OpenText("TestData/BuffCache.xml"))
                Buff.Load(fileStream);*/

            /*using (var fileStream = File.OpenText("TestData/EnchantCache.xml"))
                Enchant.Load(fileStream);*/

            /*using (var fileStream = File.OpenText("TestData/TinkeringCache.xml"))
                Tinkering.Load(fileStream);*/
//        }

        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        // [SetUp]
        public void Setup()
        {
            _character = new Character
                             {
                                 Class = CharacterClass.Priest,
                             };

            SetupCharacter(_character);
        }

        /// <summary>
        /// Implement this method to setup the character in whatever way needs to be tested
        /// </summary>
        /// <param name="character">The character.</param>
        protected abstract void SetupCharacter(Character character);

        protected void AssertStatCorrect(Func<Stats,float> property, float expected, CharacterRace race)
        {
            _character.Race = race;

            AssertStatCorrect(property, expected);
        }

        protected void AssertStatCorrect(Func<Stats, float> property, float expected)
        {
            //CalculationsShadowPriest calculations = new CalculationsShadowPriest();

            //Calculations.Models["ShadowPriest"] = typeof(CalculationsShadowPriest);
            //Calculations.LoadModel(calculations);


            //OpOv: Direct private access of fields is no longer allowed
            PrivateObject po = new PrivateObject(this, new PrivateType(typeof(Character)));
            string p = (string)po.GetField("CurrentModel");
            p = "ShadowPriest";

            //_character.CurrentModel = "ShadowPriest";

            //Stats stats = calculations.GetCharacterStats(_character, null);

            //Assert.That(property(stats), Is.EqualTo(expected).Within(0.00005f));
        }

        #endregion
    }
}