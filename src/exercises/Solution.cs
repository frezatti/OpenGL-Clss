#pragma warning disable IDE0130
namespace Exercise;

public class Solution
{
    public bool IsAnagram(string s, string t)
    {
        if (s.Length != t.Length) return false;

        int[] count = new int[26];
        for (int i = 0; i < s.Length; i++)
        {
            count[s[i] - 'a']++;
            count[t[i] - 'a']--;
        }

        for (int i = 0; i < 26; i++)
        {
            if (count[i] != 0)
            {
                return false;
            }
        }

        return true;
    }

    public int[] TwoSum(int[] nums, int target)
    {
        return nums;
    }

    public int Trap(int[] height)
    {
        var BigL = (height: 0, index: 0);
        var BigR = (height: 0, index: 0);

        foreach (var current in height)
        {

        }
        return 0;
    }
}



