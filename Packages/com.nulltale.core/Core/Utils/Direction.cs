using System;
using System.Collections.Generic;
using System.Linq;
using CoreLib;
using UnityEngine;
using Random = System.Random;

namespace CoreLib
{
    [Flags]
    public enum Direction
    {
        None   = 0,
        Center = 1,

        Left  = 1 << 1,
        Right = 1 << 2,

        Up   = 1 << 5,
        Down = 1 << 6,
    }
    
    [Flags]
    public enum DirectionCube
    {
        None   = 0,
        Center = 1,

        Left  = 1 << 1,
        Right = 1 << 2,

        Forward = 1 << 3,
        Back    = 1 << 4,

        Up   = 1 << 5,
        Down = 1 << 6,

        LeftForward  = 1 << 7,
        LeftBack     = 1 << 8,
        RightForward = 1 << 9,
        RightBack    = 1 << 10,

        LeftUp         = 1 << 11,
        RightUp        = 1 << 12,
        UpForward      = 1 << 13,
        UpBack         = 1 << 14,
        LeftUpForward  = 1 << 15,
        LeftUpBack     = 1 << 16,
        RightUpForward = 1 << 17,
        RightUpBack    = 1 << 18,

        LeftDown         = 1 << 19,
        RightDown        = 1 << 20,
        DownForward      = 1 << 21,
        DownBack         = 1 << 22,
        LeftDownForward  = 1 << 23,
        LeftDownBack     = 1 << 24,
        RightDownForward = 1 << 25,
        RightDownBack    = 1 << 26,
    }
}

public static class DirectionUtils
{
	public static readonly Dictionary<DirectionCube, Vector3Int> s_DirectionToVector3Int = new Dictionary<DirectionCube, Vector3Int>()
	{
		{ DirectionCube.Left,		new Vector3Int(-1, 0, 0) },
		{ DirectionCube.Right,		new Vector3Int(1, 0, 0) },
		{ DirectionCube.Forward,	new Vector3Int(0, 0, 1) }, 
		{ DirectionCube.Back,		new Vector3Int(0, 0, -1) },
		{ DirectionCube.Up,			new Vector3Int(0, 1, 0) }, 
		{ DirectionCube.Down,		new Vector3Int(0, -1, 0) },
		{ DirectionCube.Center,		new Vector3Int(0, 0, 0) },
		{ DirectionCube.None,		new Vector3Int(0, 0, 0) },

		{ DirectionCube.LeftForward,	new Vector3Int(-1, 0, 1) },
		{ DirectionCube.LeftBack,		new Vector3Int(-1, 0, -1) },
		{ DirectionCube.RightForward,	new Vector3Int(1, 0, 1) }, 
		{ DirectionCube.RightBack,		new Vector3Int(1, 0, -1) },
		
		{ DirectionCube.LeftUp,			new Vector3Int(-1, 1, 0) },
		{ DirectionCube.RightUp,		new Vector3Int(1, 1, 0) },
		{ DirectionCube.UpForward,		new Vector3Int(0, 1, 1) }, 
		{ DirectionCube.UpBack,			new Vector3Int(0, 1, -1) },
		{ DirectionCube.LeftUpForward,	new Vector3Int(-1, 1, 1) }, 
		{ DirectionCube.LeftUpBack,		new Vector3Int(-1, 1, -1) },
		{ DirectionCube.RightUpForward,	new Vector3Int(1, 1, 1) },
		{ DirectionCube.RightUpBack,	new Vector3Int(1, 1, -1) },
		
		{ DirectionCube.LeftDown,			new Vector3Int(-1, -1, 0) },
		{ DirectionCube.RightDown,			new Vector3Int(1, -1, 0) },
		{ DirectionCube.DownForward,		new Vector3Int(0, -1, 1) }, 
		{ DirectionCube.DownBack,			new Vector3Int(0, -1, -1) },
		{ DirectionCube.LeftDownForward,	new Vector3Int(-1, -1, 1) }, 
		{ DirectionCube.LeftDownBack,		new Vector3Int(-1, -1, -1) },
		{ DirectionCube.RightDownForward,	new Vector3Int(1, -1, 1) },
		{ DirectionCube.RightDownBack,		new Vector3Int(1, -1, -1) }
    };

	public static readonly Dictionary<DirectionCube, Vector3> s_DirectionNormal = new Dictionary<DirectionCube, Vector3>()
	{
		{ DirectionCube.Left,		new Vector3(-1, 0, 0) },
		{ DirectionCube.Right,		new Vector3(1, 0, 0) },
		{ DirectionCube.Forward,	new Vector3(0, 0, 1) }, 
		{ DirectionCube.Back,		new Vector3(0, 0, -1) },
		{ DirectionCube.Up,			new Vector3(0, 1, 0) }, 
		{ DirectionCube.Down,		new Vector3(0, -1, 0) },

		{ DirectionCube.Center,		new Vector3(0, 0, 0) },
		{ DirectionCube.None,		new Vector3(0, 0, 0) },

		{ DirectionCube.LeftForward,	new Vector3(-1, 0, 1).normalized },
		{ DirectionCube.LeftBack,		new Vector3(-1, 0, -1).normalized },
		{ DirectionCube.RightForward,	new Vector3(1, 0, 1).normalized }, 
		{ DirectionCube.RightBack,		new Vector3(1, 0, -1).normalized },
		
		{ DirectionCube.LeftUp,			new Vector3(-1, 1, 0).normalized },
		{ DirectionCube.RightUp,		new Vector3(1, 1, 0).normalized },
		{ DirectionCube.UpForward,		new Vector3(0, 1, 1).normalized }, 
		{ DirectionCube.UpBack,			new Vector3(0, 1, -1).normalized },
		{ DirectionCube.LeftUpForward,	new Vector3(-1, 1, 1).normalized }, 
		{ DirectionCube.LeftUpBack,		new Vector3(-1, 1, -1).normalized },
		{ DirectionCube.RightUpForward,	new Vector3(1, 1, 1).normalized },
		{ DirectionCube.RightUpBack,	new Vector3(1, 1, -1).normalized },
		
		{ DirectionCube.LeftDown,			new Vector3(-1, -1, 0).normalized },
		{ DirectionCube.RightDown,			new Vector3(1, -1, 0).normalized },
		{ DirectionCube.DownForward,		new Vector3(0, -1, 1).normalized }, 
		{ DirectionCube.DownBack,			new Vector3(0, -1, -1).normalized },
		{ DirectionCube.LeftDownForward,	new Vector3(-1, -1, 1).normalized }, 
		{ DirectionCube.LeftDownBack,		new Vector3(-1, -1, -1).normalized },
		{ DirectionCube.RightDownForward,	new Vector3(1, -1, 1).normalized },
		{ DirectionCube.RightDownBack,		new Vector3(1, -1, -1).normalized }
    };

	public static readonly Dictionary<DirectionCube, DirectionCube> s_DirectionRotate90_BACK_CW = new Dictionary<DirectionCube, DirectionCube>()
	{
		{ DirectionCube.Left,  DirectionCube.Up },
		{ DirectionCube.Up,	   DirectionCube.Right },
		{ DirectionCube.Right, DirectionCube.Down },
		{ DirectionCube.Down,  DirectionCube.Left },
		
		{ DirectionCube.None,   DirectionCube.None },
		{ DirectionCube.Center, DirectionCube.Center },
		
		{ DirectionCube.LeftUp,    DirectionCube.RightUp },
		{ DirectionCube.RightUp,   DirectionCube.RightDown },
		{ DirectionCube.RightDown, DirectionCube.LeftDown },
		{ DirectionCube.LeftDown,  DirectionCube.LeftUp },
    };
	
	public static readonly Dictionary<DirectionCube, DirectionCube> s_DirectionRotate90_BACK_CCW = new Dictionary<DirectionCube, DirectionCube>()
	{
		{ DirectionCube.Left,  DirectionCube.Down },
		{ DirectionCube.Down,  DirectionCube.Right },
		{ DirectionCube.Right, DirectionCube.Up },
		{ DirectionCube.Up,    DirectionCube.Left },
		
		{ DirectionCube.None,   DirectionCube.None },
		{ DirectionCube.Center, DirectionCube.Center },
		
		{ DirectionCube.LeftUp,    DirectionCube.LeftDown },
		{ DirectionCube.LeftDown,  DirectionCube.RightDown },
		{ DirectionCube.RightDown, DirectionCube.RightUp },
		{ DirectionCube.RightUp,   DirectionCube.LeftUp },
    };

	public static readonly Dictionary<DirectionCube, DirectionCube> s_DirectionRotate90_UP_CW = new Dictionary<DirectionCube, DirectionCube>()
	{
		{ DirectionCube.Left,		DirectionCube.Forward },
		{ DirectionCube.Forward,	DirectionCube.Right },
		{ DirectionCube.Right,		DirectionCube.Back },
		{ DirectionCube.Back,		DirectionCube.Left },
		
		{ DirectionCube.None,		DirectionCube.None },
		{ DirectionCube.Center,		DirectionCube.Center },
		
		{ DirectionCube.LeftForward,	DirectionCube.RightForward },
		{ DirectionCube.RightForward,	DirectionCube.RightBack },
		{ DirectionCube.RightBack,		DirectionCube.LeftBack },
		{ DirectionCube.LeftBack,		DirectionCube.LeftForward },
		
		// up
		{ DirectionCube.LeftUp,		DirectionCube.UpForward },
		{ DirectionCube.UpForward,	DirectionCube.RightUp },
		{ DirectionCube.RightUp,	DirectionCube.UpBack },
		{ DirectionCube.UpBack,		DirectionCube.LeftUp },
		
		{ DirectionCube.LeftUpForward,	DirectionCube.RightUpForward },
		{ DirectionCube.RightUpForward,	DirectionCube.RightUpBack },
		{ DirectionCube.RightUpBack,	DirectionCube.LeftUpBack },
		{ DirectionCube.LeftUpBack,		DirectionCube.LeftUpForward },
		
		// down
		{ DirectionCube.LeftDown,		DirectionCube.DownForward },
		{ DirectionCube.DownForward,	DirectionCube.RightDown },
		{ DirectionCube.RightDown,		DirectionCube.DownBack },
		{ DirectionCube.DownBack,		DirectionCube.LeftDown },
		
		{ DirectionCube.LeftDownForward,	DirectionCube.RightDownForward },
		{ DirectionCube.RightDownForward,	DirectionCube.RightDownBack },
		{ DirectionCube.RightDownBack,		DirectionCube.LeftDownBack },
		{ DirectionCube.LeftDownBack,		DirectionCube.LeftDownForward },
    };
	
	public static readonly Dictionary<DirectionCube, DirectionCube> s_DirectionRotate90_UP_CCW = new Dictionary<DirectionCube, DirectionCube>()
	{
		{ DirectionCube.Left,		DirectionCube.Back },
		{ DirectionCube.Back,		DirectionCube.Right },
		{ DirectionCube.Right,		DirectionCube.Forward },
		{ DirectionCube.Forward,	DirectionCube.Left },

		{ DirectionCube.None,		DirectionCube.None },
		{ DirectionCube.Center,		DirectionCube.Center },
		
		{ DirectionCube.LeftForward,	DirectionCube.LeftBack },
		{ DirectionCube.LeftBack,		DirectionCube.RightForward },
		{ DirectionCube.RightForward,	DirectionCube.RightBack },
		{ DirectionCube.RightBack,		DirectionCube.LeftForward },

		// up
		{ DirectionCube.LeftUp,		DirectionCube.UpBack },
		{ DirectionCube.UpBack,		DirectionCube.RightUp },
		{ DirectionCube.RightUp,	DirectionCube.UpForward },
		{ DirectionCube.UpForward,	DirectionCube.LeftUp },
		
		{ DirectionCube.LeftUpForward,	DirectionCube.LeftUpBack },
		{ DirectionCube.LeftUpBack,		DirectionCube.RightUpBack },
		{ DirectionCube.RightUpBack,	DirectionCube.RightUpForward },
		{ DirectionCube.RightUpForward,	DirectionCube.LeftUpForward },

		// down
		{ DirectionCube.LeftDown,		DirectionCube.DownBack },
		{ DirectionCube.DownBack,		DirectionCube.RightDown },
		{ DirectionCube.RightDown,		DirectionCube.DownForward },
		{ DirectionCube.DownForward,	DirectionCube.LeftDown },
		
		{ DirectionCube.LeftDownForward,	DirectionCube.LeftDownBack },
		{ DirectionCube.LeftDownBack,		DirectionCube.RightDownBack },
		{ DirectionCube.RightDownBack,		DirectionCube.RightDownForward },
		{ DirectionCube.RightDownForward,	DirectionCube.LeftDownForward },
    };

	public static readonly Dictionary<Direction, Direction> s_Rotate90_CW = new Dictionary<Direction, Direction>()
	{
		{ Direction.Left,  Direction.Up },
		{ Direction.Up,	   Direction.Right },
		{ Direction.Right, Direction.Down },
		{ Direction.Down,  Direction.Left },

		{ Direction.None,   Direction.None },
		{ Direction.Center, Direction.Center },
    };
	
	public static readonly Dictionary<Direction, Direction> s_Rotate90_CCW = new Dictionary<Direction, Direction>()
	{
		{ Direction.Left,  Direction.Down },
		{ Direction.Down,  Direction.Right },
		{ Direction.Right, Direction.Up },
		{ Direction.Up,    Direction.Left },
		
		{ Direction.None,   Direction.None },
		{ Direction.Center, Direction.Center },
    };
	
	public static readonly Dictionary<Direction, Direction> s_Invert = new Dictionary<Direction, Direction>()
	{
		{ Direction.Left,		Direction.Right },
		{ Direction.Right,		Direction.Left },
		{ Direction.Up,			Direction.Down }, 
		{ Direction.Down,		Direction.Up },
		
		{ Direction.Left | Direction.Up,    Direction.Right | Direction.Down },
		{ Direction.Left | Direction.Down,  Direction.Right | Direction.Up },
		{ Direction.Right | Direction.Up,   Direction.Right | Direction.Up },
		{ Direction.Right | Direction.Down, Direction.Left | Direction.Up },
		
		{ Direction.Center,		Direction.Center },
		{ Direction.None,		Direction.None },
    };

	public static readonly Dictionary<Direction, Vector2Int> s_ToVector2Int = new Dictionary<Direction, Vector2Int>()
	{
		{ Direction.Left,		new Vector2Int(-1, 0) },
		{ Direction.Right,		new Vector2Int(1, 0) },
		{ Direction.Up,			new Vector2Int(0, 1) }, 
		{ Direction.Down,		new Vector2Int(0, -1) },
		
		{ Direction.Left | Direction.Up,    new Vector2Int(-1, 1) },
		{ Direction.Left | Direction.Down,  new Vector2Int(-1, -1) },
		{ Direction.Right | Direction.Up,   new Vector2Int(1, 1) },
		{ Direction.Right | Direction.Down, new Vector2Int(1, -1) },
		
		{ Direction.Center,		new Vector2Int(0, 0) },
		{ Direction.None,		new Vector2Int(0, 0) },
    };

	public static readonly Dictionary<Direction, Vector2> s_ToVector2 = new Dictionary<Direction, Vector2>()
	{
		{ Direction.Left,  new Vector2(-1, 0) },
		{ Direction.Right, new Vector2(1, 0) },
		{ Direction.Up,    new Vector2(0, 1) }, 
		{ Direction.Down,  new Vector2(0, -1) },
		
		{ Direction.Left | Direction.Up,    new Vector2(-.707107f, .707107f) },
		{ Direction.Left | Direction.Down,  new Vector2(-.707107f,-.707107f) },
		{ Direction.Right | Direction.Up,   new Vector2(.707107f, .707107f) },
		{ Direction.Right | Direction.Down, new Vector2(.707107f,-.707107f) },
		
		{ Direction.Center,		new Vector2(0, 0) },
		{ Direction.None,		new Vector2(0, 0) },
    };
	
	public static readonly Dictionary<Direction, Quaternion> s_ToRotation = new Dictionary<Direction, Quaternion>()
	{
		{ Direction.Left,  Quaternion.Euler(0, 0, 180) },
		{ Direction.Right, Quaternion.Euler(0, 0, 0) },
		{ Direction.Up,    Quaternion.Euler(0, 0, 90) }, 
		{ Direction.Down,  Quaternion.Euler(0, 0, -90) },
		
		{ Direction.Left | Direction.Up,    Quaternion.Euler(0, 0, 180 - 45) },
		{ Direction.Left | Direction.Down,  Quaternion.Euler(0, 0, 180 + 45) },
		{ Direction.Right | Direction.Up,   Quaternion.Euler(0, 0, 45) },
		{ Direction.Right | Direction.Down, Quaternion.Euler(0, 0,-45) },
		
		{ Direction.Center,		Quaternion.Euler(0, 0, 0) },
		{ Direction.None,		Quaternion.Euler(0, 0, 0) },
    };

    // =======================================================================
    public static Vector3Int ToVector3Int(this DirectionCube dir)
	{
		return s_DirectionToVector3Int[dir];
	}

    public static Vector3 ToVector3(this DirectionCube dir)
	{
		return s_DirectionToVector3Int[dir].ToVector3();
	}

    public static Vector3 Normal(this DirectionCube dir)
	{
		return s_DirectionNormal[dir];
	}

    private static readonly float s_Dot45 = Mathf.Cos(Mathf.PI * .25f);

	public static Direction ToDirLRUP(this Vector2 xy)
    {
        if (xy == Vector2.zero)
			return Direction.None;
		
		xy.Normalize();

        if (Vector2.Dot(xy, Vector2.left) >= s_Dot45)
			return Direction.Left;

        if (Vector2.Dot(xy, Vector2.right) >= s_Dot45)
			return Direction.Right;

        if (Vector2.Dot(xy, Vector2.up) >= s_Dot45)
			return Direction.Up;

        if (Vector2.Dot(xy, Vector2.down) >= s_Dot45)
			return Direction.Down;

        throw new Exception("Can't happen");
    }

	public static DirectionCube Horizontal(this DirectionCube dir)
    {
		return dir & (DirectionCube.Center | DirectionCube.Left | DirectionCube.Right | DirectionCube.Forward | DirectionCube.Back | DirectionCube.LeftForward | DirectionCube.RightForward | DirectionCube.LeftBack | DirectionCube.RightBack);
    }
	
	public static DirectionCube Vertical(this DirectionCube dir)
    {
		return dir & (DirectionCube.Center | DirectionCube.Left | DirectionCube.Right | DirectionCube.Up | DirectionCube.Down | DirectionCube.LeftDown | DirectionCube.RightDown | DirectionCube.LeftUp | DirectionCube.RightUp);
    }
	
	public static DirectionCube Up(this DirectionCube dir)
    {
		return dir & (DirectionCube.Up | DirectionCube.LeftUp | DirectionCube.RightUp | DirectionCube.UpForward | DirectionCube.UpBack | DirectionCube.LeftUpForward | DirectionCube.RightUpForward | DirectionCube.LeftUpBack | DirectionCube.RightUpBack);
    }

	public static DirectionCube Down(this DirectionCube dir)
    {
		return dir & (DirectionCube.Down | DirectionCube.LeftDown | DirectionCube.RightDown | DirectionCube.DownForward | DirectionCube.DownBack | DirectionCube.LeftDownForward | DirectionCube.RightDownForward | DirectionCube.LeftDownBack | DirectionCube.RightDownBack);
    }

	public static bool HasDirection(this DirectionCube dir) => dir != DirectionCube.None && dir != DirectionCube.Center;

    public static DirectionCube Rotate90_Z_CW(this DirectionCube dir)
	{
		return s_DirectionRotate90_BACK_CW[dir];
	}

    public static DirectionCube Rotate90_Z_CCW(this DirectionCube dir)
	{
		return s_DirectionRotate90_BACK_CCW[dir];
	}

    public static DirectionCube Rotate90_Y_CW(this DirectionCube dir)
	{
		return s_DirectionRotate90_UP_CW[dir];
	}

    public static DirectionCube Rotate90_Y_CCW(this DirectionCube dir)
	{
		return s_DirectionRotate90_UP_CCW[dir];
	}
	
    public static bool IsVertical(this Direction dir)
	{
		return dir == Direction.Up || dir == Direction.Down;
	}
	
    public static bool IsHorizontal(this Direction dir)
	{
		return dir == Direction.Left || dir == Direction.Right;
	}

    public static bool HasDirection(this Direction dir)
	{
		return dir != Direction.Center && dir != Direction.None;
	}
    public static Direction Rotate90_CW(this Direction dir)
	{
		return s_Rotate90_CW[dir];
	}
    public static Direction Invert(this Direction dir)
	{
		return s_Invert[dir];
	}

    public static Direction Rotate90_CCW(this Direction dir)
	{
		return s_Rotate90_CCW[dir];
	}

    public static Vector2Int ToVector2Int(this Direction dir)
	{
		return s_ToVector2Int[dir];
	}
	
    public static Direction ToDirLRUP(this Quaternion rot)
	{
		return (rot * Vector3.right).To2DXY().ToDirLRUP();
	}

    public static Vector2 ToVector2(this Direction dir)
	{
		return s_ToVector2[dir];
	}
    public static Quaternion ToRotation2D(this Direction dir)
	{
		return s_ToRotation[dir];
	}

    public static Direction RandomLRUD()
	{
		return UnityEngine.Random.Range(0, 4) switch
		{
			0 => Direction.Left,
			1 => Direction.Right,
			2 => Direction.Up,
			3 => Direction.Down,
			_ => throw new ArgumentOutOfRangeException()
		};
	}
	
    public static Direction SimplifyLRUP(this Direction dir)
	{
		return dir & (Direction.Left | Direction.Right | Direction.Up | Direction.Down);
	}
	
    public static Direction FirstMatchLRUP(this Direction dir)
	{
		if ((dir & Direction.Left) != Direction.None)
			return Direction.Left;
		
		if ((dir & Direction.Right) != Direction.None)
			return Direction.Right;
		
		if ((dir & Direction.Up) != Direction.None)
			return Direction.Up;
		
		if ((dir & Direction.Down) != Direction.None)
			return Direction.Down;
		
		return Direction.None;
	}
	
    public static Direction ToDirection(this Vector2Int dir)
	{
		if (dir.x != 0)
			return dir.x < 0 ? Direction.Left : Direction.Right;
		if (dir.y != 0)
			return dir.y < 0 ? Direction.Down : Direction.Up;

		return Direction.None;
	}
	
	
	public static readonly KeyValuePair<Direction, Vector2>[] s_NormalsBox = new KeyValuePair<Direction, Vector2>[]
	{
		new KeyValuePair<Direction,Vector2>(Direction.Left, new Vector2(-1, 0) ),
		new KeyValuePair<Direction,Vector2>(Direction.Right, new Vector2(1, 0) ),
		new KeyValuePair<Direction,Vector2>(Direction.Up, new Vector2(0, 1) ),
		new KeyValuePair<Direction,Vector2>(Direction.Down, new Vector2(0, -1) )
	};
	
	public static readonly KeyValuePair<Direction, Vector2>[] s_Normals = new KeyValuePair<Direction, Vector2>[]
	{
		new KeyValuePair<Direction,Vector2>(Direction.Left, new Vector2(-1, 0) ),
		new KeyValuePair<Direction,Vector2>(Direction.Right, new Vector2(1, 0) ),
		new KeyValuePair<Direction,Vector2>(Direction.Up, new Vector2(0, 1) ),
		new KeyValuePair<Direction,Vector2>(Direction.Down, new Vector2(0, -1) ),
		
		new KeyValuePair<Direction, Vector2>(Direction.Left | Direction.Up, new Vector2(-0.707107f, 0.707107f)),
		new KeyValuePair<Direction, Vector2>(Direction.Left | Direction.Down, new Vector2(-0.707107f, -0.707107f)),
		new KeyValuePair<Direction, Vector2>(Direction.Right | Direction.Up, new Vector2(0.707107f, 0.707107f)),
		new KeyValuePair<Direction, Vector2>(Direction.Right | Direction.Down, new Vector2(0.707107f, -0.707107f)),
	};
	
	public static readonly KeyValuePair<Direction, Vector2>[] s_NormalsCross = new KeyValuePair<Direction, Vector2>[]
	{
		new KeyValuePair<Direction,Vector2>(Direction.Left, new Vector2(-1, 0) ),
		new KeyValuePair<Direction,Vector2>(Direction.Right, new Vector2(1, 0) ),
		new KeyValuePair<Direction,Vector2>(Direction.Up, new Vector2(0, 1) ),
		new KeyValuePair<Direction,Vector2>(Direction.Down, new Vector2(0, -1) )
	};
	
    public static Direction ToDirectionBox(this Vector2 dir)
	{
		if (dir == Vector2.zero)
			return Direction.Center;
		
		return s_NormalsBox.OrderByDescending(n => Vector2.Dot(n.Value, dir)).First().Key;
	}
	
    public static Direction ToDirectionCross(this Vector2 dir)
	{
		if (dir == Vector2.zero)
			return Direction.Center;
		
		return s_NormalsCross.OrderByDescending(n => Vector2.Dot(n.Value, dir)).First().Key;
	}
	
    public static Direction ToDirection(this Vector2 dir)
	{
		if (dir == Vector2.zero)
			return Direction.Center;
		
		return s_Normals.OrderByDescending(n => Vector2.Dot(n.Value, dir)).First().Key;
	}
	
	public static IEnumerable<Direction> Cross()
	{
		yield return Direction.Left;
		yield return Direction.Right;
		yield return Direction.Up;
		yield return Direction.Down;
	}
	
	public static IEnumerable<Direction> Box()
	{
		yield return Direction.Left;
		yield return Direction.Right;
		yield return Direction.Up;
		yield return Direction.Down;
		
		yield return Direction.Left | Direction.Up;
		yield return Direction.Right | Direction.Up;
		yield return Direction.Left | Direction.Down;
		yield return Direction.Right | Direction.Down;
	}
	
	public static IEnumerable<Direction> Coners()
	{
		yield return Direction.Left | Direction.Up;
		yield return Direction.Right | Direction.Up;
		yield return Direction.Left | Direction.Down;
		yield return Direction.Right | Direction.Down;
	}
}