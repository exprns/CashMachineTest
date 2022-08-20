using System.Text.Json;

namespace CashMachineLogic
{
    public class CashMachine
    {
        private string _pathToCashStorage;
        private Dictionary<int, int> _cashStorage = new Dictionary<int, int>();
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
                    _cashStorage = JsonSerializer.Deserialize<Dictionary<int, int>>(File.ReadAllText(_pathToCashStorage));
                    _cashStorage = _cashStorage.OrderByDescending(x => x.Key).ToDictionary(x => x.Key, y => y.Value);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public Dictionary<int, int>? WithdrawalCash(int amountToBeIssued)
        {
            return WithdrawalCash(amountToBeIssued, _cashStorage);
        }

        private Dictionary<int, int>? WithdrawalCash(int amountToBeIssued, Dictionary<int, int> cashStorage)
        {
            if (cashStorage == null || cashStorage.Count == 0)
                return null;
            var billsForWithdrawal = new Dictionary<int, int>();

            foreach (var bill in cashStorage)
            {
                if (bill.Value == 0)
                    continue;
                float maxBillCount = (float)amountToBeIssued / bill.Key;

                if (maxBillCount >= 1)
                {
                    var amount = maxBillCount > bill.Value ? bill.Value : (int)Math.Floor(maxBillCount);
                    billsForWithdrawal.Add(bill.Key, amount);
                }
            }

            foreach (var billForWithdrawal in billsForWithdrawal)
            {
                var amountToBeIssuedCur = amountToBeIssued - billForWithdrawal.Value * billForWithdrawal.Key;

                if (amountToBeIssuedCur == 0)
                {
                    return new Dictionary<int, int>(){
                        {billForWithdrawal.Key, billForWithdrawal.Value}
                    };
                }

                var cash = WithdrawalCash(amountToBeIssuedCur, cashStorage.Where(x => x.Key != billForWithdrawal.Key)
                    .ToDictionary(x => x.Key, y => y.Value));

                if (cash != null && amountToBeIssuedCur == cash.Sum(x => x.Key * x.Value))
                {
                    var correctBillsStack = new Dictionary<int, int>();
                    correctBillsStack.Add(billForWithdrawal.Key, billForWithdrawal.Value);
                    foreach (var correctBills2 in cash)
                        correctBillsStack.Add(correctBills2.Key, correctBills2.Value);
                    return correctBillsStack;
                }
            }

            return null;
        }
    }
}