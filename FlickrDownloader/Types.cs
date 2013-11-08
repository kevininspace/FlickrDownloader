// <copyright file="Program.cs" company="Sondre Bjellås">
// This software is licensed as Microsoft Public License (Ms-PL).
// </copyright>
//-----------------------------------------------------------------------

// TODO: Refactor all of these types into individual files.

namespace FlickrDownloader
{
    public enum SearchType
    {
        All,
        Favorites,
        Contacts,
        Recent,
        Tag,
        Group,
        Search,
        Selection
    }

    public class SearchOptions
    {
        public SourceOptions Source { get; set; }
        public string Text { get; set; }
        public SearchType Type { get; set; }
        //public string Set { get; set; }
        public string Path { get; set; }
        //public string UserId { get; set; }
    }

    public enum SourceOptions
    {
        User,
        Group,
        Search
    }
}