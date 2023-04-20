public class TestUtil {
        public static string RemoveLF(string input)
        {
            string output = input.Replace("\n", "").Replace("\r", "");
            return output;
        }
}