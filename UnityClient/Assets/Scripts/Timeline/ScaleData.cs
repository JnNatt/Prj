public class ScaleData
{
    public int Start;
    public int Gap;

    public int Middle
    {
        get { return Start + Gap / 2; }
    }

    public int End
    {
        get { return Start + Gap; }
    }
}