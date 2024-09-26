using System;
using System.Linq;
using NUnit.Framework;
using SaveLoad.Tests.Mocks;
using UnityEngine;

namespace SaveLoad.Tests
{
    [TestFixture]
    public class DataStorageTests
    {
        private const string TEST_SAVE_KEY = "TEST_STORAGE";
        
        private GameDataStorage _dataStorage;
        private SerializableDataMock _serializableDataMock;
        
        [OneTimeSetUp]
        public void SetUp()
        {
            var saveSettings = ScriptableObject.CreateInstance<SaveLoadSettings>();
            saveSettings.SetKey(TEST_SAVE_KEY);
            
            _dataStorage = new GameDataStorage(new SaveLoadSerializerFactory(), saveSettings);
            _serializableDataMock = new SerializableDataMock();
            _serializableDataMock.Init();
            
            _dataStorage.RemoveSaves(TEST_SAVE_KEY);
            _dataStorage.SetData(_serializableDataMock.CurrenciesData);
            _dataStorage.SaveState(TEST_SAVE_KEY);
        }

        [TestCase("gold", 30f, ExpectedResult = true)]
        [TestCase("gold", 40f, ExpectedResult = false)]
        [TestCase("wood", 0f, ExpectedResult = false)]
        [TestCase("wood", 5f, ExpectedResult = false)]
        [TestCase("wood", 100f, ExpectedResult = true)]
        [TestCase("soft", 10f, ExpectedResult = true)]
        [TestCase("soft", 20f, ExpectedResult = false)]
        [TestCase("soft", 30f, ExpectedResult = false)]
        public bool GetCurrencyAmount(string key, float value)
        {
            var data = _dataStorage.GetData<TestCurrenciesData>();
            var amount = data.CurrencyDatas
                .FirstOrDefault(d => d.Type == key).Amount;
            
            return Math.Abs(amount - value) < 0.01f;
        }
        
        [TestCase("gold", ExpectedResult = true)]
        [TestCase("apple", ExpectedResult = false)]
        [TestCase("sword", ExpectedResult = false)]
        [TestCase("wood", ExpectedResult = true)]
        [TestCase("soft", ExpectedResult = true)]
        [TestCase("meat", ExpectedResult = false)]
        public bool TryGetCurrency(string key)
        {
            if (_dataStorage.TryGetData<TestCurrenciesData>(out var data))
            {
                return data.CurrencyDatas.Any(d => d.Type == key);
            }

            return false;
        }
        
        [Test]
        public void AddAndSaveState()
        {
            _dataStorage.SetData(_serializableDataMock.CharacterUpgradesData);
            _dataStorage.SaveState(TEST_SAVE_KEY);
            _dataStorage.TryGetData<TestCharacterUpgradesData>(out var data);
            Assert.IsTrue(data != default);
        }
        
        [OneTimeTearDown] 
        public void ClearTests()
        {
            _dataStorage.RemoveSaves(TEST_SAVE_KEY);
            _dataStorage = null;
            _serializableDataMock = null;
        }
    }
}