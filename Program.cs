
using CashMachineLogic;

var path = "CashStorage.json";
var cashMachine = new CashMachine(path);


while (true)
{
    cashMachine.UpdateCashStorage();

    Console.WriteLine("Write amount of cash for issue!");
    var amountToBeIssuedStr = Console.ReadLine();

    if (amountToBeIssuedStr == null || amountToBeIssuedStr == "")
        break;
    if (Int32.TryParse(amountToBeIssuedStr, out var amountToBeIssued))
    {
        var cash = cashMachine.WithdrawalCash(amountToBeIssued);

        if (cash != null)
        {
            foreach (var bill in cash)
            {
                Console.WriteLine($"{bill.Value} - {bill.Key}");
            }
        }
        else
        {
            Console.WriteLine("Can't issue this sum(");
        }
    }
}