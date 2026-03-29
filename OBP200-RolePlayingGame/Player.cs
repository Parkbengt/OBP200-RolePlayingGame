namespace OBP200_RolePlayingGame;

public class Player
{
    public string Name { get; set; }
    public IPlayerClass Class { get; set; }

    public int Hp { get; private set; }
    public int MaxHp { get; private set; }
    public int Atk { get; set; }
    public int Def { get; set; }
    public int Gold { get; set; }
    public int Xp { get; private set; }
    public int Level { get; private set; }
    public int Potions { get; set; }
    public List<string> Inventory { get; set; }

    public bool IsDead => Hp <= 0;

    public Player(string name, IPlayerClass @class)
    {
        Name = name;
        Class = @class;
        MaxHp = @class.StartingMaxHp;
        Hp = MaxHp;
        Atk = @class.StartingAtk;
        Def = @class.StartingDef;
        Potions = @class.StartingPotions;
        Gold = @class.StartingGold;
        Xp = 0;
        Level = 1;
        Inventory = new List<string> { "Wooden Sword", "Cloth Armor" };
    }

    public void TakeDamage(int amount)
    {
        Hp = Math.Max(0, Hp - Math.Max(0, amount));
    }

    public void UsePotion()
    {
        if (Potions <= 0)
        {
            Console.WriteLine("Du har inga drycker kvar.");
            return;
        }
        int heal = 12;
        int newHp = Math.Min(MaxHp, Hp + heal);
        Console.WriteLine($"Du dricker en dryck och återfår {newHp - Hp} HP.");
        Hp = newHp;
        Potions--;
    }

    public void AddXp(int amount)
    {
        Xp += Math.Max(0, amount);
        MaybeLevelUp();
    }

    public void AddGold(int amount)
    {
        Gold += Math.Max(0, amount);
    }

    public void AddToInventory(string item)
    {
        Inventory.Add(item);
    }

    public void ShowStatus()
    {
        Console.WriteLine(
            $"[{Name} | {Class.ClassName}]  " +
            $"HP {Hp}/{MaxHp}  ATK {Atk}  DEF {Def}  " +
            $"LVL {Level}  XP {Xp}  Guld {Gold}  Drycker {Potions}");
        if (Inventory.Count > 0)
            Console.WriteLine($"Väska: {string.Join(";", Inventory)}");
    }

    private void MaybeLevelUp()
    {
        int nextThreshold = Level == 1 ? 10 : (Level == 2 ? 25 : (Level == 3 ? 45 : Level * 20));
        if (Xp < nextThreshold) return;

        Level++;
        var (hpBonus, atkBonus, defBonus) = Class.GetLevelUpBonuses();
        MaxHp += hpBonus;
        Atk += atkBonus;
        Def += defBonus;
        Hp = MaxHp;

        Console.WriteLine($"Du når nivå {Level}! Värden ökade och HP återställd.");
    }
    // Behövde lägga till detta eftersom Hp nu har privat set och DoRest() inte kunde "heala" annars //
    public void RestoreFullHp()
    {
        Hp = MaxHp;
    }
}