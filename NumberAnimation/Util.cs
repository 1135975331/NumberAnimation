using System;
using System.Globalization;
using System.Linq;

namespace NumberAnimation;

public static class Util
{
	public static string DigitGroupSeparator(double num)
	{
		if(num >= Math.Pow(10, 15))
			return num.ToString(CultureInfo.InvariantCulture);
        
		var numStrList = num.ToString(CultureInfo.InvariantCulture).ToList();
		for(var i = 3; i < numStrList.Count; i += 4)
			numStrList.Insert(numStrList.Count - i, ',');

		return string.Join("", numStrList);
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="min">최솟값</param>
	/// <param name="value">값</param>
	/// <param name="max">최댓값</param>
	/// <returns>value가 min보다 작으면 min 반환, max보다 크다면 max 반환, 둘다 아니라면 value 그대로 반환</returns>
	public static int MinMax(int min, int value, int max)
	{
		if(min > value)
			return min;

		if(value > max)
			return max;

		return value;
	}
}