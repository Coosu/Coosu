namespace OSharp.Shared.IO
{
    public static class File
    {
        public static string EscapeFileName(string source)
        {
            return source.Replace(@"\", "").Replace(@"/", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "")
                .Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
        }
    }
}
