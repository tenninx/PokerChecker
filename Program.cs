internal class Program
{
    static readonly HashSet<string> Symbols = ["S", "H", "D", "C"];

    private static void Main(string[] args)
    {
        Console.WriteLine("""
            Poker Result Checker

            The symbol on the cards can be inputted as follows:
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

            string _input = Console.ReadLine();

            if (!_input.ToLower().Equals("x"))
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

        return GetHand(_cards);
    }

    private static clsCard[] ValidateInputAndParseCards(string input)
    {
        input = input.ToString().Trim().Replace(" ", "").ToUpper();
        // Split the input into 5 distinct cards
        string[] _cards = input.Split(",").Distinct().ToArray();

        if (_cards.Length != 5) return null;

        clsCard[] _objCards = new clsCard[5];
        string _symbol;

        for (int i = 0; i < _cards.Length; i++)
        {
            _symbol = _cards[i].Substring(0, 1);
            if (!Symbols.Contains(_symbol)) return null;

            if (!int.TryParse(_cards[i].Substring(1), out int _number) || _number < 0 || _number > 13) return null;

            _objCards[i] = new clsCard { Symbol = _symbol, Number = _number };
        }

        // Sort the cards according to the Number so that it is easier to check straight
        Array.Sort(_objCards, delegate (clsCard x, clsCard y) { return x.Number.CompareTo(y.Number); });

        return _objCards;
    }

    private static string GetHand(clsCard[] input)
    {
        clsProcessedCardsStatus _processed = new clsProcessedCardsStatus();

        string _lastSymbol = input[0].Symbol;
        int _lastNumber = input[0].Number;

        // Order is correct for Royal Flush but further check on straight needed
        _processed.IsRoyalFlushCandidate = input[0].Number == 1 ? true : false;

        for (int i = 0; i < input.Length; i++)
        {
            if (_processed.IsSameSymbol && input[i].Symbol == _lastSymbol)
                _lastSymbol = input[i].Symbol;
            else
                _processed.IsSameSymbol = false;
            // Special case for Royal Flush
            if (_processed.IsStraight && i == 0 && input[0].Number == 1)
                _lastNumber = 10;
            else if (_processed.IsStraight && input[i].Number != _lastNumber++)
                _processed.IsStraight = false;

            if (!_processed.ProcessedCards.ContainsKey(input[i].Number))
                _processed.ProcessedCards.Add(input[i].Number, 1);
            else
                _processed.ProcessedCards[input[i].Number]++;
        }

        return GetResult(_processed);
    }

    private static string GetResult(clsProcessedCardsStatus processed)
    {
        // Find the max number of times a Number appears in the collection
        int maxOccurrences = processed.ProcessedCards.Max(num => num.Value);

        if (processed.ProcessedCards.Count == 5 && processed.IsSameSymbol && processed.IsStraight && processed.IsRoyalFlushCandidate)
            return "1. Royal Flush: All cards are of the same suit. Number must be 10, J, Q, K, and A correspondingly.";
        if (processed.ProcessedCards.Count == 5 && processed.IsSameSymbol && processed.IsStraight)
            return "2. Straight Flush: All cards are of the same suit. Numbers of the five cards must be continuous.";
        if (maxOccurrences == 4)
            return "3. Four-of-a-Kind: Four out of five cards must have the same number.";
        if (maxOccurrences == 3 && processed.ProcessedCards.Count == 2)
            return "4. Full House: Three out of five cards must have the same number and the remaining two must have the same number accordingly.";
        if (processed.IsSameSymbol)
            return "5. Flush: All cards are of the same suit.";
        if (processed.IsStraight)
            return "6. Straight: Numbers of the five cards must be continuous and not all cards are of the same suit.";
        if (maxOccurrences == 3 && processed.ProcessedCards.Count == 3)
            return "7. Three-of-a-Kind: Three out of five cards must have the same number.";
        if (processed.ProcessedCards.Count == 3)
            return "8. Two Pair: There must be two pairs where a pair means two cards which have the same number.";
        if (processed.ProcessedCards.Count == 4)
            return "9. One Pair: There must be a pair where a pair means two cards which have the same number.";

        return "10. Nothing: the cards don't match any of the hands above";
    }

    /// <summary>
    /// An individual card.
    /// </summary>
    public class clsCard
    {
        public string Symbol { get; set; }
        public int Number { get; set; }
    }

    /// <summary>
    /// Stores the states of the card collection.
    /// </summary>
    public class clsProcessedCardsStatus()
    {
        // True if the Numbers on all cards in the collection are in a sequence
        public bool IsStraight { get; set; } = true;

        // True if the Symbols on all cards in the collection are the same
        public bool IsSameSymbol { get; set; } = true;

        // True if the first sorted card is an Ace (Number 1). IsStraight and IsSameSymbol must be True for this flag to be considered.
        public bool IsRoyalFlushCandidate { get; set; } = true;

        public Dictionary<int, int> ProcessedCards = new Dictionary<int, int>();
    }
}