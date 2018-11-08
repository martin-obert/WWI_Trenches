namespace Assets.SpCrsVrPrototypes
{
    public interface IEntityMapper
    {
        int NameToId(string name);

        string IdToName(int id);

        int MapName(string name);
    }
}