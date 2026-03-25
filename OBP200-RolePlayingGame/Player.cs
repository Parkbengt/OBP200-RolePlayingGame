namespace OBP200_RolePlayingGame;

public class Player
{
    public string Name { get; set; }
    public IPlayerClass Class { get; set; }
    
    public int Hp { get; set; }
    public int MaxHp { get; set; }
    public int Atk { get; set; }
    public int Def { get; set; }   
    public int Gold { get; set; }
    public int Xp { get; set; }   
    public int Level { get; set; }
    public int Potions { get; set; }
    public List<string> Inventory { get; set; } = new List<string>();
    
    public bool IsDead => Hp <= 0;
    
}