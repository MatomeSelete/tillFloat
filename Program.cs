using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        string inputFile = "input.txt";
        string outputFile = "output.txt";

        ProcessTransactions(inputFile, outputFile);
    }

    static void ProcessTransactions(string inputFile, string outputFile)
    {
        Dictionary<int, int> till = new Dictionary<int, int>()
        {
            { 50, 5 },
            { 20, 5 },
            { 10, 6 },
            { 5, 12 },
            { 2, 10 },
            { 1, 10 }
        };

        int tillBalance = 500;
        StreamWriter writer = new StreamWriter(outputFile);

        writer.WriteLine("Till Start, Transaction Total, Paid, Change Total, Change Breakdown");

        string[] lines = File.ReadAllLines(inputFile);

        foreach (string line in lines)
        {
            string[] parts = line.Split(';');
            string[] items = parts[0].Split(',');
            int transactionTotal = 0;

            foreach (string item in items)
            {
                string[] itemParts = item.Split(' ');
                string lastPart = itemParts[itemParts.Length - 1];
                int amount = int.Parse(lastPart.Substring(1));
                transactionTotal += amount;
            }

            int amountPaid = parts[1].Split('-').Sum(amount => int.Parse(amount.TrimStart('R')));
            int changeTotal = amountPaid - transactionTotal;

            List<string> changeBreakdown = new List<string>();
            foreach (KeyValuePair<int, int> kvp in till.OrderByDescending(kv => kv.Key))
            {
                int denomination = kvp.Key;
                int count = kvp.Value;

                int numNotes = changeTotal / denomination;
                numNotes = Math.Min(numNotes, count);

                if (numNotes > 0)
                {
                    changeBreakdown.Add($"{numNotes}xR{denomination}");
                    changeTotal -= numNotes * denomination;
                    till[denomination] -= numNotes;
                }

                if (changeTotal == 0)
                    break;
            }

            writer.WriteLine($"R{tillBalance}, R{transactionTotal}, R{amountPaid}, R{changeTotal}, {string.Join("-", changeBreakdown)}");

            tillBalance += transactionTotal;
        }

        writer.WriteLine($"R{tillBalance}");

        writer.Close();
    }
}
