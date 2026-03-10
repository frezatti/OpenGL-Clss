using Xunit;
using Exercise;

namespace Exercise.Test;

public class UnitTest1
{
    [Fact]
    public void Trapped_Water_Exercise()
    {
        var solution = new Solution();

        int[] height = [0, 2, 0, 3, 1, 0, 1, 3, 2, 1];
        int expectedArea = 9;

        int calculatedArea = solution.Trap(height);

        Assert.Equal(expectedArea, calculatedArea);
    }
}
