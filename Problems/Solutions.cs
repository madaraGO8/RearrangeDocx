﻿using System;
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
                        op.Add(temp);
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
            for (int i = 0; i < s.Length; i++)
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
            try
            {
                if (strs == null || strs.Length == 0)
                {
                    return "";
                }
                int minLength = strs.Min(s => s.Length);
                StringBuilder a = new StringBuilder();
                for (int i = 0; i < minLength; i++)
                {
                    char firstStrChar = strs[0][i];
                    bool allMatch = true;
                    for (int j = 1; j < strs.Length; j++)
                    {
                        if (strs[j][i] != firstStrChar)
                        {
                            allMatch = false;
                            break;
                        }
                    }
                    if (allMatch)
                        a.Append(firstStrChar);
                    else
                        break;
                }
                return a.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
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
                arr[sentence[i] - 'a'] += 1;
            }
            for (int j = 0; j < arr.Length; j++)
            {
                if (arr[j] == 0)
                    return false;

            }
            return true;
        }
        public bool IsValid(string s)
        {
            Stack<char> stack = new Stack<char>();
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '(' || s[i] == '[' || s[i] == '{')
                {
                    stack.Push(s[i]);
                }
                else
                {
                    if (stack.Count() == 0)
                        return false;
                    char pop = stack.Pop();
                    if ((s[i] == '}' && pop != '{') || (s[i] == ']' && pop != '[') || (s[i] == ')' && pop != '('))
                    {
                        return false;
                    }
                }
            }
            if (stack.Count > 0)
                return false;
            return true;
        }
        public void Pattern()
        {
            #region Low to high
            for (int i = 1; i < 4; i++)
            {
                int j = 0;
                while (j < i)
                {
                    Console.Write("*");
                    j++;
                }
                Console.WriteLine();
            }
            #endregion
            #region High to low
            for (int i = 3; i > 0; i--)
            {
                int j = i;
                while (j > 0)
                {
                    Console.Write("*");
                    j--;
                }
                Console.WriteLine();
            }
            #endregion
        }
        public void OddEve()
        {
            int number = 6;
            string binary = Convert.ToString(number, 2);
            if (binary.EndsWith("0"))
            {
                Console.WriteLine(number + " " + "is Even");
            }
            else
            {
                Console.WriteLine(number + " " + "is Odd");
            }
        }


    }
}

