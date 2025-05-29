namespace DiceEmulator.Entities;

public class Dice
{
    public int[] Faces { get; }
    public int Id { get; }

    public Dice(int id, int[] faces)
    {
        Id = id;
        Faces = faces;
    }

    public override string ToString() => $"[{string.Join(",", Faces)}]";
}