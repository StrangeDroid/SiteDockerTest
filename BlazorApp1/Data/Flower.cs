public class Flower
{
    public int Id { get; set; }
    public int Hp { get; set; } = 15;
    public int MaxHp { get; set; } = 15;
    public int Slot { get; set; } // pot_slot
    public int TypeId { get; set; } // ID из flowertypes

    public string CodeName { get; set; } = ""; // из flowertypes.code
    public string DisplayName { get; set; } = ""; // из flowertypes.name
    public int PriceBuy { get; set; }  // ← из flowertypes
    public int PriceSell { get; set; } // ← из flowertypes
    public string GetStateImage()
    {
        if (IsDead)
            return $"/images/flowers/{CodeName}/fall.png"; // или любой другой файл

        string imageFile = Hp switch
        {
            15 => "rise.png",
            >= 12 => "pre_rise.png",
            >= 9 => "standart.png",
            >= 3 => "pre_fall.png",
            >= 1 => "fall.png",
            _ => "fall.png"
        };

        return $"/images/flowers/{CodeName}/{imageFile}";
    }

    public bool IsDead => Hp <= 0;
}
