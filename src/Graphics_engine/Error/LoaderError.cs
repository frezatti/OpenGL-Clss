public enum LoaderErrorCode
{
    None,
    FileNotFound,
    EmptyFile,
    InvalidFieldCount,
    InvalidFloat,
    FileReadingError,
}

public readonly record struct LoaderError(
    LoaderErrorCode Code,
    string Message,
    int LineNumber = -1)
{
    public static readonly LoaderError None =
        new(LoaderErrorCode.None, string.Empty);
}
