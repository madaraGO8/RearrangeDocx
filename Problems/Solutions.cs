using System;
using System.Collections.Generic;
using System.Linq;
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
                    if (nums[i] + nums[temp] == target)
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
        public int RomanToInt(string s)
        {
            int a = 0;
            Dictionary<char, int> romanNumbers = new Dictionary<char, int>
            {
                {'I', 1},
                {'V', 5},
                {'X', 10},
                {'L', 50},
                {'C', 100},
                {'D', 500},
                {'M', 1000}
            };
            for (int i = 0; i<s.Length; i++)
            {
                if (i + 1 < s.Length && romanNumbers[s[i]] < romanNumbers[s[i + 1]])
                {
                    a -= romanNumbers[s[i]];
                }
                else
                {
                    a += romanNumbers[s[i]];
                }
            }
            return a;
        }
        public string LongestCommonPrefix(string[] strs)
        {
            string a = "";
            List<string> inp = new List<string>();
            inp.AddRange(strs);
            int stringIndex = 0;
            int charIndex = 0;
            while (stringIndex < inp.Count)
            {
                string currentString = inp[stringIndex];
                char letter = currentString[charIndex];
                charIndex++;

                if (charIndex >= currentString.Length)
                {
                    stringIndex++;
                    charIndex = 0;
                }
            }
            return a;
        }
        public bool CheckIfPangram(string sentence)
        {
            if (sentence == null || sentence.Length == 0)
            {
                return false;
            }
            int[] arr = new int[26];
            for (int i = 0; i < sentence.Length; i++)
            {
                arr[sentence[i] - 97] += 1;
            }
            for (int j = 0; j < arr.Length; j++)
            {
                if (arr[j] == 0)
                    return false;
                
            }
            return true;
        }
    }
}

