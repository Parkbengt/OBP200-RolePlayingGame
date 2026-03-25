namespace OBP200_RolePlayingGame;

public class PlayerClassWarrior
{
    public string ClassName => "Warrior";
    public int StartingMaxHp => 40;
    public int StartingAtk => 7;
    public int StartingDef => 5;
    public int StartingPotions => 2;
    public int StartingGold => 15;
    public double FleeChance => 0.25;
    
    public int GetAttackBonus(int baseAtk, int enemyDef, Random rng)
    {
        int baseDmg = Math.Max(1, baseAtk - (enemyDef / 2));
        return Math.Max(1, baseDmg + 1 + rng.Next(0, 3)); // +1 warrior buff
    }

    string cls = Player.Class ?? "Warrior";
    int specialDmg = 0;

    public int UseClassSpecial(Player player, int enemyDef, bool vsBoss, Random rng)
    {
        // Heavy Strike: hög skada men självskada
        Console.WriteLine("Warrior använder Heavy Strike!");

        int atk = player.Atk;
        int specialDmg = Math.Max(2, atk + 3 - enemyDef);
        ApplyDamageToPlayer(2); // självskada
        return specialDmg;
    }

    public (int HpBonus, int AtkBonus, int DefBonus) GetLevelUpBonuses() => (6, 2, 2);
}