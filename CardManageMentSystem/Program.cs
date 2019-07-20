using System;
using System.Collections.Generic;

namespace CardManageMentSystem
{
    public abstract class Card
    {
        private int cardNumber;
        private int pinNumber;
        private string cardType;
        private Transaction transaction;
        public Card(string cardType, int cardNumber, int pinNumber)
        {
            this.cardType = cardType;
            this.cardNumber = cardNumber;
            this.pinNumber = pinNumber;
            transaction = new Transaction();
        }

        public int CardNumber
        {
            get { return cardNumber; }
        }

        public int PinNumber
        {
            get { return pinNumber; }
        }

        public string CardType
        {
            get { return cardType; }
        }
        public void SaveTransaction(int amount, string typeOfTransaction)
        {
            transaction.AddTransaction(amount, typeOfTransaction);
        }

        public void ShowTransactionDetails()
        {
            transaction.ShowAll();
        }
        abstract public string MakePayment(int amount);
        abstract public string Deposit(int amount);
        abstract public int GetUsableAmount();

    }

    public class PrepaidCard : Card
    {
        private int balance;
        public PrepaidCard(int cardNumber, int pinNumber) :
            base("Prepaid", cardNumber, pinNumber)
        {
            balance = 0;
        }

        public int Balance
        {
            get { return balance; }
        }
        public override string MakePayment(int amount)
        {
            if (amount <= balance)
            {
                balance -= amount;
                this.SaveTransaction(amount, "Credit");
                return "Payment successfull";
            }

            return "Don't have enough balance. Deposit balance first";
        }

        public override string Deposit(int amount)
        {
            this.SaveTransaction(amount, "Debit");
            balance += amount;

            return "Deposit successfull";
        }

        public override int GetUsableAmount()
        {
            return balance;
        }
    }

    public class CreditCard : Card
    {
        private int maxLimit;
        private int loan;
        public CreditCard(int cardNumber, int pinNumber, int limit = 10000) :
            base("Credit", cardNumber, pinNumber)
        {
            loan = 0;
            maxLimit = limit;
        }
        public override string MakePayment(int amount)
        {
            if (amount + loan <= maxLimit)
            {
                loan += amount;
                this.SaveTransaction(amount, "Credit");
                return "Payment Successful";
            }
            return "You have crossed your limit! Payment failed";
        }

        public override string Deposit(int amount)
        {
            loan -= amount;
            this.SaveTransaction(amount, "Dedit");
            return "Deposit successfull";
        }

        public override int GetUsableAmount()
        {
            return maxLimit - loan;
        }
    }

    public class TransactionDetails
    {
        public DateTime dateTime;
        public string TransacationType;
        public int amount;
    }
    public class Transaction
    {
        private List<TransactionDetails> transactions;
        public Transaction()
        {
            transactions = new List<TransactionDetails>();
        }

        public void AddTransaction(int amount, string typeOfTransaction)
        {
            transactions.Add(new TransactionDetails()
            {
                dateTime = DateTime.Now,
                amount = amount,
                TransacationType = typeOfTransaction
            });

        }
        public void ShowAll()
        {
            foreach (TransactionDetails transaction in transactions)
            {
                if (transaction.TransacationType == "Debit")
                {
                    Console.WriteLine($"You have added {transaction.amount} in your account" +
                        $" on {transaction.dateTime.ToString()}");

                }
                else
                {
                    Console.WriteLine($"You have spend {transaction.amount} from your account" +
                        $" on {transaction.dateTime.ToString()}");
                }
            }
        }

    }

    public class CardController
    {
        private List<Card> cards;
        public CardController()
        {
            cards = new List<Card>();
        }

        public string AddCard(string cardType)
        {
            KeyValuePair<int, int> number = this.GenerateCardNumberAndPinNumber();
            int cardNumber = number.Key;
            int pinNumber = number.Value;

            if (cardType == "Prepaid")
            {
                cards.Add(new PrepaidCard(cardNumber, pinNumber));
            }
            else if (cardType == "Credit")
            {
                cards.Add(new CreditCard(cardNumber, pinNumber));
            }

            return "Your card Number: " + cardNumber.ToString() + "\nPin Number: " + pinNumber.ToString();
        }

        public Card GetInstanceOfCard(int cardNumber, int pinNumber)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].CardNumber == cardNumber && cards[i].PinNumber == pinNumber)
                {
                    return cards[i];
                }
            }

            return null;
        }

        private KeyValuePair<int, int> GenerateCardNumberAndPinNumber()
        {
            int sz = cards.Count;   /// better to write algorithm to generate unique card and pin number
            return new KeyValuePair<int, int>(sz + 1, sz + 1);
        }
    }
    class Program
    {
        public static void Clear()
        {
            Console.WriteLine("Press anykey to clear");
            Console.ReadKey();
            Console.Clear();
        }
        static void Main(string[] args)
        {
            CardController cardController = new CardController();
            while (true)
            {
                Console.WriteLine("Welcome to card management system");
                Console.WriteLine("To Apply for new card press 1");
                Console.WriteLine("To Enter your card press 2");

                int input = Convert.ToInt32(Console.ReadLine());
                switch (input)
                {
                    case 1:
                        while (true)
                        {
                            Console.WriteLine("1: Prepaid Card");
                            Console.WriteLine("2: Credit Card");
                            Console.WriteLine("3: Exit");
                            int cardInput = Convert.ToInt32(Console.ReadLine());
                            if (cardInput == 1)
                            {
                                string credential = cardController.AddCard("Prepaid");
                                Console.WriteLine(credential);
                                Clear();
                                break;

                            }
                            else if (cardInput == 2)
                            {
                                string credential = cardController.AddCard("Credit");
                                Console.WriteLine(credential);
                                Clear();
                                break;
                            }
                            else if (cardInput == 3)
                            {
                                Console.Clear();
                                break;
                            }
                            else
                            {
                                Console.WriteLine("You have pressed wrong. Press again");
                            }
                        }
                        break;
                    case 2:
                        Console.WriteLine("Give your card number");
                        int cardNumber = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Give your pin number");
                        int pinNumber = Convert.ToInt32(Console.ReadLine());
                        Card card = cardController.GetInstanceOfCard(cardNumber, pinNumber);

                        if (card == null)
                        {
                            Console.WriteLine("You have entered wrong credential");
                            Clear();
                            break;
                        }
                        else
                        {

                            while (true)
                            {
                                Console.WriteLine(card.CardType == "Prepaid" ? "1: Check Balance" : "1: Check Current Limit");
                                Console.WriteLine("2: Make Payment");
                                Console.WriteLine("3: Deposit Money");
                                Console.WriteLine("4: Transaction Details");
                                Console.WriteLine("5: Exit");

                                int userInput = Convert.ToInt32(Console.ReadLine());

                                if (userInput == 1)
                                {
                                    Console.Clear();
                                    if (card.CardType == "Prepaid")
                                    {
                                        Console.WriteLine("Your balance is " + card.GetUsableAmount());
                                    }
                                    else
                                    {
                                        Console.WriteLine("Your Current Limit is " + card.GetUsableAmount());
                                    }
                                    Clear();
                                }
                                else if (userInput == 2)
                                {
                                    Console.Clear();
                                    Console.WriteLine("Enter amount");
                                    int amount = Convert.ToInt32(Console.ReadLine());
                                    Console.WriteLine(card.MakePayment(amount));
                                    Clear();
                                }
                                else if (userInput == 3)
                                {
                                    Console.Clear();
                                    Console.WriteLine("Enter amount");
                                    int amount = Convert.ToInt32(Console.ReadLine());
                                    Console.WriteLine(card.Deposit(amount));
                                    Clear();

                                }
                                else if (userInput == 4)
                                {
                                    Console.Clear();
                                    card.ShowTransactionDetails();
                                    Clear();
                                }
                                else
                                {
                                    Console.Clear();
                                    break;
                                }
                            }
                        }
                        break;
                    default:
                        Console.WriteLine("You have pressed wrong. Press again");
                        break;
                }
            }
        }
    }
}
