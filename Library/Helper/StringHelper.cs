namespace Library.Helper;

public static class StringHelper
{
    public static string RemoveLastJpeg(string fileName)
    {
        const string extension = ".jpeg";

        if (fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
        {
            return fileName.Substring(0, fileName.Length - extension.Length);
        }

        return fileName;
    }
}
