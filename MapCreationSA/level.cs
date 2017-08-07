namespace MapCreationSA
{
    public class level
    {
        public string height { get; set; }
        public int arrIndex { get; set; }

        public level(string _height, int _index)
        {
            this.height = _height;
            this.arrIndex = _index;
        }

        public override string ToString()
        {
            return height;
        }
    }
}
