using System;

namespace Problems
{
    class Program
    {
        public static int[] nums = { 3,2,3 };
        public static int target = 6;
        public static int x = 123321;
        public static string s = "DXLIX";
        public static string[] pre = { "flower", "flop", "flight" };
        public static string sentence = "thequickbrownfoxjumpsoverthelazydog";
        public static string bracs = "{()}";
        static void Main(string[] args)
        {
            new Solutions().TwoSum(nums, target);
            new Solutions().IsPalindrome(x);
            new Solutions().RomanToInt(s);
            new Solutions().LongestCommonPrefix(pre);
            new Solutions().CheckIfPangram(sentence);
            new Solutions().IsValid(bracs);
        }
    }
}
