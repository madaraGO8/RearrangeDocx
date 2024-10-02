using System;

namespace Problems
{
    class Program
    {
        public static int[] nums = { 3,2,3 };
        public static int target = 6;
        public static int x = 123321;
        static void Main(string[] args)
        {
            new Solutions().TwoSum(nums, target);
            new Solutions().IsPalindrome(x);
        }
    }
}
