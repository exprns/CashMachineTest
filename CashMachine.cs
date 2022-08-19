using System.Text.Json;

namespace CashMachineLogic
{
    public class CashMachine
    {
        private string _pathToCashStorage;
        private Dictionary<int, int> cashStorage = new Dictionary<int, int>();
        public CashMachine(string pathToCashStorage)
        {
            _pathToCashStorage = pathToCashStorage;
        }

        public void UpdateCashStorage()
        {
            if (File.Exists(_pathToCashStorage))
            {
                try
                {
                    cashStorage = JsonSerializer.Deserialize<Dictionary<int, int>>(File.ReadAllText(_pathToCashStorage));
                    cashStorage = cashStorage.OrderByDescending(x => x.Key).ToDictionary(x => x.Key, y => y.Value);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public Dictionary<int, int> GetCash(int sum)
        {
            var result = new Dictionary<int, int>();
            foreach (var interval in cashStorage)
            {
                int count = sum / interval.Key;

                if (count != 0 && interval.Value != 0)
                {
                    var amount = count > interval.Value ? interval.Value : count;
                    sum = sum - amount * interval.Key;
                    result.Add(interval.Key, amount);
                }

            }

            return result;
        }
    }
}