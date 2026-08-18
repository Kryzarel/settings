using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Kryz.Settings
{
	public static class FNVHash
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<byte> GetBytes<T>(T value) where T : struct
		{
			ReadOnlySpan<T> span = MemoryMarshal.CreateReadOnlySpan(ref value, 1);
			ReadOnlySpan<byte> bytes = MemoryMarshal.AsBytes(span);
			return bytes;
		}

		public static class UInt64
		{
			private const ulong offsetBasis = 14695981039346656037;
			private const ulong prime = 1099511628211;

			public static ulong Hash<T>(T value) where T : struct
			{
				return Hash(GetBytes(value));
			}

			public static ulong Hash(ReadOnlySpan<byte> bytes)
			{
				ulong hash = offsetBasis;

				foreach (byte b in bytes)
				{
					hash ^= b;
					hash *= prime;
				}
				return hash;
			}
		}

		public static class UInt32
		{
			private const uint offsetBasis = 2166136261;
			private const uint prime = 16777619;

			public static uint Hash<T>(T value) where T : struct
			{
				return Hash(GetBytes(value));
			}

			public static uint Hash(ReadOnlySpan<byte> bytes)
			{
				uint hash = offsetBasis;

				foreach (byte b in bytes)
				{
					hash ^= b;
					hash *= prime;
				}
				return hash;
			}
		}
	}
}