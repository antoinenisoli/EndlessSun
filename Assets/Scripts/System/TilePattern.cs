using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileDirection
{
	MiddleLeft,
	Middle,
	MiddleRight,

	UpLeft,
	UpMiddle,
	UpRight,

	DownLeft,
	DownMiddle,
	DownRight,
}

public class TilePattern
{
	static Dictionary<TileDirection, bool[,]> storedPatterns = new Dictionary<TileDirection, bool[,]>()
	{
		{ TileDirection.MiddleLeft, 
			new bool[3, 3]
			{
				{ false, false, false },
				{ true, false, true },
				{ true, true, true },
			}
		},

		{ TileDirection.Middle,
			new bool[3, 3]
			{
				{ true, true, true },
				{ true, false, true },
				{ true, true, true },
			}
		},

		{ TileDirection.MiddleRight, 
			new bool[3, 3]
			{
				{ true, true, true },
				{ true, false, true },
				{ false, false, false },
			}
		},

		{ TileDirection.UpLeft,
			new bool[3, 3]
			{
				{ false, false, false },
				{ true, false, false },
				{ true, true, false },
			}
		},

		{ TileDirection.UpMiddle,
			new bool[3, 3]
			{
				{ true, true, false },
				{ true, false, false },
				{ true, true, false },
			}
		},

		{ TileDirection.UpRight,
			new bool[3, 3]
			{
				{ true, true, false },
				{ true, false, false },
				{ false, false, false },
			}
		},

		{ TileDirection.DownLeft,
			new bool[3, 3]
			{
				{ false, false, false },
				{ false, false, true },
				{ false, true, true },
			}
		},

		{ TileDirection.DownMiddle,
			new bool[3, 3]
			{
				{ false, true, true },
				{ false, false, true },
				{ false, true, true },
			}
		},

		{ TileDirection.DownRight,
			new bool[3, 3]
			{
				{ false, true, true },
				{ false, false, true },
				{ false, false, false },
			}
		},
	};

	public static string WriteMatrix(bool[,] matrix)
	{
		string output = " ";
		for (int x = 0; x <= matrix.GetLength(0) - 1; x++)
			for (int y = 0; y <= matrix.GetLength(1) - 1; y++)
				output += " " + matrix[x, y].GetHashCode() + " ";

		return output;
	}

	public static int GetPatternIndex(bool[,] matrix)
    {
		var array = Enum.GetValues(typeof(TileDirection));
        for (int i = 0; i < array.Length; i++)
        {
			if (Compare((TileDirection)array.GetValue(i), matrix))
				return i;
		}

		return 1;
    }

	static bool Compare(TileDirection direction, bool[,] matrix)
	{
		for (int x = 0; x <= matrix.GetLength(0) - 1; x++)
			for (int y = 0; y <= matrix.GetLength(1) - 1; y++)
            {
				if (storedPatterns.TryGetValue(direction, out bool[,] value))
				{
					//MonoBehaviour.print(WriteMatrix(value) + " lol " + direction);
					if (matrix[x, y] != value[x, y])
						return false;
				}
				else
					return false;
            }

		return true;
	}
}
