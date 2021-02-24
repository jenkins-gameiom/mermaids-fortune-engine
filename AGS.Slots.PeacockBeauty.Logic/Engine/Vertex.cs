namespace AGS.Slots.MermaidsFortune.Logic
{
    public class Vertex<T>
    {
        public T Item { get; set; }
        public bool Visited { get; set; }
        public int RefCount { get; set; }

        public Vertex(T item)
        {
            this.Item = item;
        }

        public override bool Equals(object obj)
        {
            return this.Item.Equals(((Vertex<T>)obj).Item);
        }

        public override int GetHashCode()
        {
            return this.Item.GetHashCode();
        }


    }
}
