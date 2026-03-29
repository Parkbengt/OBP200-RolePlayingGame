namespace OBP200_RolePlayingGame;

public class Warrior : IPlayerClass
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
        return Math.Max(1, baseDmg + 1 + rng.Next(0, 3));
    }

    public int UseClassSpecial(Player player, int enemyDef, bool vsBoss, Random rng)
    {
        Console.WriteLine("Warrior använder Heavy Strike!");
        int dmg = Math.Max(2, player.Atk + 3 - enemyDef);
        player.TakeDamage(2);
        int finalDmg = vsBoss ? (int)Math.Round(dmg * 0.8) : dmg;
        return Math.Max(0, finalDmg);
    }

    public (int hp, int atk, int def) GetLevelUpBonuses() => (6, 2, 2);
}