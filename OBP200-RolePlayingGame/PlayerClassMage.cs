namespace OBP200_RolePlayingGame;

public class Mage : IPlayerClass
{
    public string ClassName => "Mage";
    public int StartingMaxHp => 28;
    public int StartingAtk => 10;
    public int StartingDef => 2;
    public int StartingPotions => 2;
    public int StartingGold => 15;
    public double FleeChance => 0.35;

    public int GetAttackBonus(int baseAtk, int enemyDef, Random rng)
    {
        int baseDmg = Math.Max(1, baseAtk - (enemyDef / 2));
        return Math.Max(1, baseDmg + 2 + rng.Next(0, 3));
    }

    public int UseClassSpecial(Player player, int enemyDef, bool vsBoss, Random rng)
    {
        if (player.Gold < 3)
        {
            Console.WriteLine("Inte tillräckligt med guld för att kasta Fireball (kostar 3).");
            return 0;
        }

        Console.WriteLine("Mage kastar Fireball!");
        player.Gold -= 3;
        int dmg = Math.Max(3, player.Atk + 5 - (enemyDef / 2));
        int finalDmg = vsBoss ? (int)Math.Round(dmg * 0.8) : dmg;
        return Math.Max(0, finalDmg);
    }

    public (int hp, int atk, int def) GetLevelUpBonuses() => (4, 4, 1);
}