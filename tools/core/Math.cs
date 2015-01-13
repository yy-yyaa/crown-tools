/*
 * Copyright (c) 2012-2014 Daniele Bartolini and individual contributors.
 * License: https://github.com/taylor001/crown/blob/master/LICENSE
 */

namespace Crown
{
namespace Core
{
	public struct Vector2
	{
		public Vector2(double x, double y)
		{
			this.x = x;
			this.y = y;
		}

		public static Vector2 operator+(Vector2 a, Vector2 b)
		{
			return new Vector2(a.x + b.x, a.y + b.y);
		}

		public static Vector2 operator-(Vector2 a, Vector2 b)
		{
			return new Vector2(a.x - b.x, a.y - b.y);
		}

		public static Vector2 operator*(Vector2 a, double k)
		{
			return new Vector2(a.x * k, a.y * k);
		}

		public static Vector2 operator*(double k, Vector2 a)
		{
			return a * k;
		}

		public double x, y;
	}

	public struct Vector3
	{
		public Vector3(double x, double y, double z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public static Vector3 operator+(Vector3 a, Vector3 b)
		{
			return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
		}

		public static Vector3 operator-(Vector3 a, Vector3 b)
		{
			return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		public static Vector3 operator*(Vector3 a, double k)
		{
			return new Vector3(a.x * k, a.y * k, a.z * k);
		}

		public static Vector3 operator*(double k, Vector3 a)
		{
			return a * k;
		}

		public double x, y, z;
	}

	public struct Quaternion
	{
		public Quaternion(double x, double y, double z, double w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public double x, y, z, w;
	}

	public struct AABB
	{
		public Vector2 min, max;
	}
} // namespace Core
} // namespace Crown
