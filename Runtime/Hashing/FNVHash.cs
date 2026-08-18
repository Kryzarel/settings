using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Kryz.Settings
{
	public static class FNVHash
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<byte> GetBytes(int value)
		{
			ReadOnlySpan<int> ints = MemoryMarshal.CreateReadOnlySpan(ref value, 1);
			ReadOnlySpan<byte> bytes = MemoryMarshal.AsBytes(ints);
			return bytes;
		}

		public static class Hash64
		{
			private const ulong offsetBasis = 14695981039346656037;
			private const ulong prime = 0x100000001b3;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static ulong Hash(ReadOnlySpan<byte> bytes)
			{
				ulong hash = offsetBasis;

				foreach (byte b in bytes)
				{
					hash = (hash ^ b) * prime;
				}
				return hash;
			}
		}

		public static class Hash32
		{
			private const uint offsetBasis = 2166136261;
			private const uint prime = 16777619;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static uint Hash(ReadOnlySpan<byte> bytes)
			{
				uint hash = offsetBasis;

				foreach (byte b in bytes)
				{
					hash = (hash ^ b) * prime;
				}
				return hash;
			}
		}
	}
}