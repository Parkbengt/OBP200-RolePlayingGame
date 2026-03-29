
using System.Text;

namespace OBP200_RolePlayingGame;

class Program
{
    private static Player _currentPlayer = null!;
    
    private static List<string[]> _rooms = new List<string[]>();
    private static List<string[]> _enemyTemplates = new List<string[]>();
    private static int _currentRoomIndex;
    private static Random _rng = new Random();

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        InitEnemyTemplates();

        while (true)
        {
            ShowMainMenu();
            Console.Write("Välj: ");
            var choice = (Console.ReadLine() ?? "").Trim();

            if (choice == "1")
            {
                StartNewGame();
                RunGameLoop();
            }
            else if (choice == "2")
            {
                Console.WriteLine("Avslutar...");
                return;
            }
            else
            {
                Console.WriteLine("Ogiltigt val.");
            }

            Console.WriteLine();
        }
    }

    // ======= Meny & Init =======

    static void ShowMainMenu()
    {
        Console.WriteLine("=== Text-RPG ===");
        Console.WriteLine("1. Nytt spel");
        Console.WriteLine("2. Avsluta");
    }

    static void StartNewGame()
    {
        Console.Write("Ange namn: ");
        var name = (Console.ReadLine() ?? "").Trim();
        if (string.IsNullOrWhiteSpace(name)) name = "Namnlös";

        Console.WriteLine("Välj klass: 1) Warrior  2) Mage  3) Rogue");
        Console.Write("Val: ");
        var k = (Console.ReadLine() ?? "").Trim();
        
        IPlayerClass chosenClass = k switch
        {
            "1" => new Warrior(),
            "2" => new Mage(),
            "3" => new Rogue(),
            _   => new Warrior()
        };
        
        _currentPlayer = new Player(name, chosenClass);

        _rooms.Clear();
        _rooms.Add(new[] { "battle",   "Skogsstig" });
        _rooms.Add(new[] { "treasure", "Gammal kista" });
        _rooms.Add(new[] { "shop",     "Vandrande köpman" });
        _rooms.Add(new[] { "battle",   "Grottans mynning" });
        _rooms.Add(new[] { "rest",     "Lägereld" });
        _rooms.Add(new[] { "battle",   "Grottans djup" });
        _rooms.Add(new[] { "boss",     "Urdraken" });

        _currentRoomIndex = 0;

        Console.WriteLine($"Välkommen, {_currentPlayer.Name} the {_currentPlayer.Class.ClassName}!");
        _currentPlayer.ShowStatus();
    }

    static void RunGameLoop()
    {
        while (true)
        {
            var room = _rooms[_currentRoomIndex];
            Console.WriteLine($"--- Rum {_currentRoomIndex + 1}/{_rooms.Count}: {room[1]} ({room[0]}) ---");

            bool continueAdventure = EnterRoom(room[0]);

            if (_currentPlayer.IsDead)
            {
                Console.WriteLine("Du har stupat... Spelet över.");
                break;
            }

            if (!continueAdventure)
            {
                Console.WriteLine("Du lämnar äventyret för nu.");
                break;
            }

            _currentRoomIndex++;

            if (_currentRoomIndex >= _rooms.Count)
            {
                Console.WriteLine();
                Console.WriteLine("Du har klarat äventyret!");
                break;
            }

            Console.WriteLine();
            Console.WriteLine("[C] Fortsätt     [Q] Avsluta till huvudmeny");
            Console.Write("Val: ");
            var post = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();

            if (post == "Q")
            {
                Console.WriteLine("Tillbaka till huvudmenyn.");
                break;
            }

            Console.WriteLine();
        }
    }

    // ======= Rumshantering =======

    static bool EnterRoom(string type)
    {
        switch ((type ?? "battle").Trim())
        {
            case "battle":
                return DoBattle(isBoss: false);
            case "boss":
                return DoBattle(isBoss: true);
            case "treasure":
                return DoTreasure();
            case "shop":
                return DoShop();
            case "rest":
                return DoRest();
            default:
                Console.WriteLine("Du vandrar vidare...");
                return true;
        }
    }

    // ======= Strid =======

    static bool DoBattle(bool isBoss)
    {
        var enemy = GenerateEnemy(isBoss);
        Console.WriteLine($"En {enemy[1]} dyker upp! (HP {enemy[2]}, ATK {enemy[3]}, DEF {enemy[4]})");

        int enemyHp  = ParseInt(enemy[2], 10);
        int enemyAtk = ParseInt(enemy[3], 3);
        int enemyDef = ParseInt(enemy[4], 0);

        while (enemyHp > 0 && !_currentPlayer.IsDead)
        {
            Console.WriteLine();
            _currentPlayer.ShowStatus();
            Console.WriteLine($"Fiende: {enemy[1]} HP={enemyHp}");
            Console.WriteLine("[A] Attack   [X] Special   [P] Dryck   [R] Fly");
            if (isBoss) Console.WriteLine("(Du kan inte fly från en boss!)");
            Console.Write("Val: ");

            var cmd = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();

            if (cmd == "A")
            {
                int damage = _currentPlayer.Class.GetAttackBonus(_currentPlayer.Atk, enemyDef, _rng);
                enemyHp -= damage;
                Console.WriteLine($"Du slog {enemy[1]} för {damage} skada.");
            }
            else if (cmd == "X")
            {
                int special = _currentPlayer.Class.UseClassSpecial(_currentPlayer, enemyDef, isBoss, _rng);
                enemyHp -= special;
                Console.WriteLine($"Special! {enemy[1]} tar {special} skada.");
            }
            else if (cmd == "P")
            {
                _currentPlayer.UsePotion();
            }
            else if (cmd == "R" && !isBoss)
            {
                if (_rng.NextDouble() < _currentPlayer.Class.FleeChance)
                {
                    Console.WriteLine("Du flydde!");
                    return true;
                }
                else
                {
                    Console.WriteLine("Misslyckad flykt!");
                }
            }
            else
            {
                Console.WriteLine("Du tvekar...");
            }

            if (enemyHp <= 0) break;

            // Fiendens tur
            int enemyDamage = CalculateEnemyDamage(enemyAtk);
            _currentPlayer.TakeDamage(enemyDamage);
            Console.WriteLine($"{enemy[1]} anfaller och gör {enemyDamage} skada!");
        }

        if (_currentPlayer.IsDead) return false;

        // Vinstrapporter, XP, guld, loot
        int xpReward = ParseInt(enemy[5], 5);
        int goldReward = ParseInt(enemy[6], 3);
        
        _currentPlayer.AddXp(xpReward);
        _currentPlayer.AddGold(goldReward);

        Console.WriteLine($"Seger! +{xpReward} XP, +{goldReward} guld.");
        MaybeDropLoot(enemy[1]);

        return true;
    }

    static string[] GenerateEnemy(bool isBoss)
    {
        if (isBoss)
            return new[] { "boss", "Urdraken", "55", "9", "4", "30", "50" };

        var template = _enemyTemplates[_rng.Next(_enemyTemplates.Count)];
        int hp   = ParseInt(template[2], 10) + _rng.Next(-1, 3);
        int atk  = ParseInt(template[3], 3)  + _rng.Next(0, 2);
        int def  = ParseInt(template[4], 0)  + _rng.Next(0, 2);
        int xp   = ParseInt(template[5], 4)  + _rng.Next(0, 3);
        int gold = ParseInt(template[6], 2)  + _rng.Next(0, 3);
        return new[] { template[0], template[1], hp.ToString(), atk.ToString(), def.ToString(), xp.ToString(), gold.ToString() };
    }

    static void InitEnemyTemplates()
    {
        _enemyTemplates.Clear();
        _enemyTemplates.Add(new[] { "beast",  "Vildsvin",  "18", "4", "1", "6", "4" });
        _enemyTemplates.Add(new[] { "undead", "Skelett",   "20", "5", "2", "7", "5" });
        _enemyTemplates.Add(new[] { "bandit", "Bandit",    "16", "6", "1", "8", "6" });
        _enemyTemplates.Add(new[] { "slime",  "Geléslem",  "14", "3", "0", "5", "3" });
    }

    static int CalculateEnemyDamage(int enemyAtk)
    {
        int def  = _currentPlayer.Def;
        int roll = _rng.Next(0, 3);
        int dmg  = Math.Max(1, enemyAtk - (def / 2)) + roll;
        if (_rng.NextDouble() < 0.1) dmg = Math.Max(1, dmg - 2);
        return dmg;
    }

    static void MaybeDropLoot(string enemyName)
    {
        if (_rng.NextDouble() < 0.35)
        {
            string item = enemyName.Contains("Urdraken") ? "Dragon Scale" : "Minor Gem";
            _currentPlayer.AddToInventory(item);
            Console.WriteLine($"Föremål hittat: {item} (lagt i din väska)");
        }
    }

    static bool DoTreasure()
    {
        Console.WriteLine("Du hittar en gammal kista...");
        if (_rng.NextDouble() < 0.5)
        {
            int gold = _rng.Next(8, 15);
            _currentPlayer.AddGold(gold);
            Console.WriteLine($"Kistan innehåller {gold} guld!");
        }
        else
        {
            var items = new[] { "Iron Dagger", "Oak Staff", "Leather Vest", "Healing Herb" };
            string found = items[_rng.Next(items.Length)];
            _currentPlayer.AddToInventory(found);
            Console.WriteLine($"Du plockar upp: {found}");
        }
        return true;
    }

    static bool DoShop()
    {
        Console.WriteLine("En vandrande köpman erbjuder sina varor:");
        while (true)
        {
            Console.WriteLine($"Guld: {_currentPlayer.Gold} | Drycker: {_currentPlayer.Potions}");
            Console.WriteLine("1) Köp dryck (10 guld)");
            Console.WriteLine("2) Köp vapen (+2 ATK) (25 guld)");
            Console.WriteLine("3) Köp rustning (+2 DEF) (25 guld)");
            Console.WriteLine("4) Sälj alla 'Minor Gem' (+5 guld/st)");
            Console.WriteLine("5) Lämna butiken");
            Console.Write("Val: ");
            var val = (Console.ReadLine() ?? "").Trim();

            if (val == "1")
                TryBuy(10, () => _currentPlayer.Potions++, "Du köper en dryck.");
            else if (val == "2")
                TryBuy(25, () => _currentPlayer.Atk += 2, "Du köper ett bättre vapen.");
            else if (val == "3")
                TryBuy(25, () => _currentPlayer.Def += 2, "Du köper bättre rustning.");
            else if (val == "4")
                SellMinorGems();
            else if (val == "5")
            {
                Console.WriteLine("Du säger adjö till köpmannen.");
                break;
            }
            else
                Console.WriteLine("Köpmannen förstår inte ditt val.");
        }
        return true;
    }

    static void TryBuy(int cost, Action apply, string successMsg)
    {
        if (_currentPlayer.Gold >= cost)
        {
            _currentPlayer.Gold -= cost;
            apply();
            Console.WriteLine(successMsg);
        }
        else
        {
            Console.WriteLine("Du har inte råd.");
        }
    }

    static void SellMinorGems()
    {
        int count = _currentPlayer.Inventory.Count(x => x == "Minor Gem");
        if (count == 0)
        {
            Console.WriteLine("Inga 'Minor Gem' i väskan.");
            return;
        }
        _currentPlayer.Inventory.RemoveAll(x => x == "Minor Gem");
        _currentPlayer.AddGold(count * 5);
        Console.WriteLine($"Du säljer {count} st Minor Gem för {count * 5} guld.");
    }

    static bool DoRest()
    {
        Console.WriteLine("Du slår läger och vilar.");
        _currentPlayer.RestoreFullHp();
        Console.WriteLine("HP återställt till max.");
        return true;
    }

    static int ParseInt(string s, int fallback)
    {
        try
        {
            int value = Convert.ToInt32(s);
            return value;
        }
        catch (Exception e)
        {
            return fallback;
        }
    }
}
