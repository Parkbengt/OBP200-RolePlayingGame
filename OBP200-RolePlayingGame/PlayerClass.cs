namespace OBP200_RolePlayingGame;

public interface IPlayerClass
{
    string ClassName { get; }
    int StartingMaxHp { get; }
    int StartingAtk { get; }
    int StartingDef { get; }
    int StartingPotions { get; }
    int StartingGold { get; }
    double FleeChance { get; }
    int GetAttackBonus(int baseAtk, int enemyDef, Random rng);
    int UseSpecial(int baseAtk, int enemyDef, bool vsBoss, Random rng);
    (int hp, int atk, int def) GetLevelUpBonuses();
}