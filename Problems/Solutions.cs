using System;
using System.Collections.Generic;
using System.Text;

namespace Problems
{
    class Solutions
    {
        //[3,2,3]
        public int[] TwoSum(int[] nums, int target)
        {
            List<int> op = new List<int>();
            for (int i = 0; i <= nums.Length - 1; i++)
            {
                var temp = i + 1;
                while (temp <= nums.Length - 1)
                {
                    if(nums[i] + nums[temp] == target)
                    {
                        op.Add(i);
                        op.Add(i + 1);
                        return op.ToArray();
                    }
                    else
                    {
                        temp++;
                    }
                }
            }
            return op.ToArray();
        }
        public bool IsPalindrome(int x)
        {
            bool isPalindrome = true;
            string inp = x.ToString();
            var a = inp.Length;
            int b = 0;
            if (a % 2 == 0)
            {
                b = a / 2;
                int stringIndex = b - 1;
                var midPoint = inp[stringIndex];
                var partA = inp.Substring(0, stringIndex + 1);
                var partB = inp.Substring(stringIndex + 1);
                char[] rev = partB.ToCharArray();
                Array.Reverse(rev);
                partB = new string(rev);
                if (partA == partB)
                {
                    isPalindrome = true;
                }
                else
                    isPalindrome = false;
            }
            else if (a % 2 != 0)
            {
                b = a / 2;
                b += 1;
                int stringIndex = b - 1;
                var midPoint = inp[stringIndex];
                var partA = inp.Substring(0, stringIndex);
                var partB = inp.Substring(stringIndex + 1);
                char[] rev = partB.ToCharArray();
                Array.Reverse(rev);
                partB = new string(rev);
                if (partA == partB)
                {
                    isPalindrome = true;
                }
                else
                    isPalindrome = false;
            }
            return isPalindrome;
        }
    }
}

