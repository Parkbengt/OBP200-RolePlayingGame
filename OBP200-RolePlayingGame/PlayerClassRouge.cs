namespace OBP200_RolePlayingGame;

public class Rogue : IPlayerClass
{
    public string ClassName => "Rogue";
    public int StartingMaxHp => 32;
    public int StartingAtk => 8;
    public int StartingDef => 3;
    public int StartingPotions => 3;
    public int StartingGold => 20;
    public double FleeChance => 0.5;

    public int GetAttackBonus(int baseAtk, int enemyDef, Random rng)
    {
        int baseDmg = Math.Max(1, baseAtk - (enemyDef / 2));
        int critBonus = rng.NextDouble() < 0.2 ? 4 : 0;
        return Math.Max(1, baseDmg + critBonus + rng.Next(0, 3));
    }

    public int UseClassSpecial(Player player, int enemyDef, bool vsBoss, Random rng)
    {
        int dmg;
        if (rng.NextDouble() < 0.5)
        {
            Console.WriteLine("Rogue utför en lyckad Backstab!");
            dmg = Math.Max(4, player.Atk + 6);
        }
        else
        {
            Console.WriteLine("Backstab misslyckades!");
            dmg = 1;
        }

        int finalDmg = vsBoss ? (int)Math.Round(dmg * 0.8) : dmg;
        return Math.Max(0, finalDmg);
    }

    public (int hp, int atk, int def) GetLevelUpBonuses() => (5, 3, 1);
}