namespace Tfs2Gource.Extensions {
    public static class ByteExtensions {
        public static string AsString(this byte[] bytes) {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }
}
