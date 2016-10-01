using SQLite;

namespace MyFirstProject
{
	public class Entry
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public int Place { get; set; }
        public int Appid { get; set; }
    }
}