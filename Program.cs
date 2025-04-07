internal class Program
{
    static readonly HashSet<string> Suits = ["S", "H", "D", "C"];

    private static void Main(string[] args)
    {
        Console.WriteLine("""
            Poker Result Checker

            The suit on the cards can be inputted as follows:
            S = ♠ Spade
            H = ♥ Heart
            D = ♦ Diamond
            C = ♣ Club

            The number on the cards can be inputted as follows:
            A    = 1
            2-10 = 2-10
            J    = 11
            Q    = 12
            K    = 13

            The combination of 5 cards can result in one of the following "Hands":
            
            1. Royal Flush: All cards are of the same suit. Number must be 10, J, Q, K, and A. Example: [♠10, ♠J, ♠Q, ♠K, ♠A], [♦10, ♦J, ♦Q, ♦K, ♦A]
            2. Straight Flush: All cards are of the same suit. Numbers of the five cards must be continuous. Example: [♠9, ♠10, ♠J, ♠Q, ♠K], [♦4, ♦5, ♦6, ♦7, ♦8]
            3. Four-of-a-Kind: Four out of five cards must have the same number. Example: [2,2,2,2,3], [6,6,6,6,5]
            4. Full House: Three out of five cards must have the same number and the remaining two must have the same number accordingly. Example: [3,3,3,2,2], [7,7,7,J,J]
            5. Flush: All cards are of the same suit. Example:[♠2, ♠J, ♠5, ♠7, ♠3], [♦10, ♦6, ♦3, ♦K, ♦A]
            6. Straight: Numbers of the five cards must be continuous and not all cards are of the same suit. Example: [4,5,6,7,8], [8,9,10,J,Q]
            7. Three-of-a-Kind: Three out of five cards must have the same number. Example: [3,3,3,6,7],[Q,Q,Q,4,8]
            8. Two Pair: There must be two pairs where a pair means two cards which have the same number. Example: [2,2,3,3,6], [7,7,4,4,K]
            9. One Pair: There must be a pair where a pair means two cards which have the same number. Example: [2,2,5,8,K], [Q,Q,7,8,10]
            10. Nothing: the cards don't match any of the hands above

            Input of 5 cards can be entered as a comma-separated string. Here is an example of the input:
            H5,S13,C7,C1,D11
            """);

        while (true)
        {
            Console.WriteLine("\nEnter the combination of 5 cards to check, enter 'x' to terminate:");

            string _input = Console.ReadLine().ToUpper();

            if (!_input.Equals("X"))
                Console.WriteLine("The result: {0}", ProcessCards(_input));
            else
                break;
        }
    }

    private static string ProcessCards(string input)
    {
        clsCard[] _cards = ValidateInputAndParseCards(input);

        if (_cards == null)
            return "Error Input";

        return GetHandType(_cards);
    }

    private static clsCard[] ValidateInputAndParseCards(string input)
    {
        input = input.ToString().Trim().Replace(" ", "");
        // Split the input into 5 distinct cards
        string[] _cards = input.Split(",").Distinct().ToArray();

        if (_cards.Length != 5) return null;

        clsCard[] _objCards = new clsCard[5];
        string _suit;

        for (int i = 0; i < _cards.Length; i++)
        {
            _suit = _cards[i].Substring(0, 1);
            if (!Suits.Contains(_suit)) return null;

            if (!int.TryParse(_cards[i].Substring(1), out int _number) || _number < 0 || _number > 13) return null;

            _objCards[i] = new clsCard { Suit = _suit, Number = _number };
        }

        // Sort the cards according to the Number so that it is easier to check straight
        Array.Sort(_objCards, delegate (clsCard x, clsCard y) { return x.Number.CompareTo(y.Number); });

        return _objCards;
    }

    private static string GetHandType(clsCard[] input)
    {
        int _max = input.GroupBy(c => c.Number).Max(d => d.Count());
        int _count = input.Select(c => c.Number).Distinct().Count();
        bool isFlush = input.Select(c => c.Suit).Distinct().Count() == 1;
        bool isRoyalFlush = isFlush && _count == 5 && input[0].Number == 1 ? input[1].Number == 10 && input[4].Number == 13 : false;
        bool isStraight = !isRoyalFlush && _count == 5 ? input[0].Number == 1 ? input[1].Number == 10 && input[4].Number == 13 : input[4].Number - input[0].Number == 4 : false;

        if (isRoyalFlush)
            return "1. Royal Flush: All cards are of the same suit. Number must be 10, J, Q, K, and A correspondingly.";
        if (isFlush && isStraight)
            return "2. Straight Flush: All cards are of the same suit. Numbers of the five cards must be continuous.";
        if (_max == 4)
            return "3. Four-of-a-Kind: Four out of five cards must have the same number.";
        if (_max == 3 && _count == 2)
            return "4. Full House: Three out of five cards must have the same number and the remaining two must have the same number accordingly.";
        if (isFlush)
            return "5. Flush: All cards are of the same suit.";
        if (isStraight)
            return "6. Straight: Numbers of the five cards must be continuous and not all cards are of the same suit.";
        if (_max == 3 && _count == 3)
            return "7. Three-of-a-Kind: Three out of five cards must have the same number.";
        if (_count == 3)
            return "8. Two Pair: There must be two pairs where a pair means two cards which have the same number.";
        if (_count == 4)
            return "9. One Pair: There must be a pair where a pair means two cards which have the same number.";

        return "10. Nothing: the cards don't match any of the hands above";
    }

    /// <summary>
    /// An individual card.
    /// </summary>
    public class clsCard
    {
        public string Suit { get; set; }
        public int Number { get; set; }
    }
}